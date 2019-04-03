using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Amazon.DocSamples.S3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkietbaanBE.Lib;
using SkietbaanBE.Models;
using SkietbaanBE.RequestModel;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/Scores")]

    public class ScoresController : Controller
    {
        private S3Client s3client;

        private ModelsContext _context;
        public ScoresController(ModelsContext context)
        {
            _context = context;
        }

        // GET: api/Scores
        [HttpGet("{token}")]
        public async System.Threading.Tasks.Task<List<Score>> GetScores(string token)
        {
            User userid = _context.Users.FirstOrDefault(x => x.Token.Equals(token));
            var scoreList = _context.Scores.Where(x => x.User.Id == userid.Id);
            List<Score> scoreLists = new List<Score>();
            s3client = new S3Client();
            foreach (var score in scoreList)
            {
                if (score.PictureURL.Contains(userid.Token))
                {
                    var key = $"{userid.Token}-{score.Id}";
                    var ImageString = await s3client.GetObjectString(key);
                    if (!string.IsNullOrEmpty(ImageString))
                    {
                        ImageString = "data:image / png; base64," + DecompressString(ImageString);
                    }
                    score.PictureURL = ImageString;
                }
                scoreLists.Add(score);
            }
            return scoreLists;
        }


        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> ScoreCapture([FromBody]ScoreCapture scoreCapture)
        {
            if (ModelState.IsValid)
            {
                var competition = _context.Competitions.Where(x => x.Name == scoreCapture.CompetitionName).FirstOrDefault<Competition>();
                if (competition == null)
                {
                    return new NotFoundObjectResult("Competition not found");
                }
                var user = _context.Users.Where(x => x.Token == scoreCapture.Token).FirstOrDefault<User>();
                if (user == null)
                {
                    return new NotFoundObjectResult("User not found");
                }
                var key = "";
                if(!string.IsNullOrEmpty(scoreCapture.PictureURL))
                {
                    var LastSavedScore = _context.Scores.LastOrDefault(x => x.User == user);
                    key = $"{scoreCapture.Token}-{LastSavedScore?.Id}";
                }
                var score = new Score()
                {
                    UserScore = scoreCapture.UserScore,
                    PictureURL = key,
                    Competition = competition,
                    User = user,
                    Latitude = scoreCapture.Latitude,
                    Longitude = scoreCapture.Longitude,
                    UploadDate = DateTime.Now
                };


                _context.Scores.Add(score);
                _context.SaveChanges();
                if (!string.IsNullOrEmpty(scoreCapture.PictureURL))
                {
                    s3client = new S3Client();

                    var bytes = Convert.FromBase64String(CompressString(scoreCapture.PictureURL.Replace("data:image/png;base64,", "")));
                    Stream contents = new MemoryStream(bytes);
                    try
                    {
                        await s3client.SaveObjectAsync(contents, key);
                    }
                    catch (Exception e)
                    {
                        var test = e;
                    }
                }


                //update time spent at skietbaan
                CheckAward.UpdateHoursSpent(_context, score);

                //update User Competition Stats
                Calculations calculations = new Calculations(_context);
                calculations.performCalculations(user.Id, competition.Id);

                return Ok("Score Added Successfully");
            }
            else
            {
                return new BadRequestObjectResult("score cannot be null");
            }
        }
        public static string CompressString(string text)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(text);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
            {
                gZipStream.Write(buffer, 0, buffer.Length);
            }

            memoryStream.Position = 0;

            var compressedData = new byte[memoryStream.Length];
            memoryStream.Read(compressedData, 0, compressedData.Length);

            var gZipBuffer = new byte[compressedData.Length + 4];
            Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
            return Convert.ToBase64String(gZipBuffer);
        }
        public static string DecompressString(string compressedText)
        {
            byte[] gZipBuffer = Convert.FromBase64String(compressedText);
            using (var memoryStream = new MemoryStream())
            {
                int dataLength = BitConverter.ToInt32(gZipBuffer, 0);
                memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

                var buffer = new byte[dataLength];

                memoryStream.Position = 0;
                using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
                {
                    gZipStream.Read(buffer, 0, buffer.Length);
                }

                return Encoding.UTF8.GetString(buffer);
            }
        }
    }
 
}