﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoCAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Cors;
using System.Diagnostics;
using Newtonsoft.Json.Linq;

namespace CoCAPI.Controllers
{
    //[Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class CoCController : ControllerBase
    {

        [HttpGet]
        [Route("api/coc")]
        public IEnumerable<Question> Get([FromQuery]string ProgramStartDate, [FromQuery]string EntryDate, [FromQuery]string DateOfService, [FromQuery]string Client, [FromQuery]int TreatmentType)
        {
            string echo = "N/A";
            string exclude = "";
            bool ShowQuestions = false;
            List<Question> lQuestions = new List<Question>();
            try
            {
                if (TreatmentType == 0) // New
                { }
                else if (TreatmentType == 1) // Extension
                {
                    exclude = "";
                    ShowQuestions = true;
                }
                else if (TreatmentType == 2) // Continuation
                {
                    DateTime dEntryDate = DateTime.Parse(EntryDate);
                    DateTime dDateOfService = DateTime.Parse(DateOfService);
                    DateTime dProgramStartDate = DateTime.Parse(ProgramStartDate);
                    DateTime dProgramStartDatePlus180 = dProgramStartDate.AddDays(180);
                    DateTime dProgramStartDateMinus180 = dProgramStartDate.AddDays(-180);
                    if (dEntryDate <= dProgramStartDatePlus180)
                    {
                        exclude = "345";
                        ShowQuestions = true;
                    }
                    else if (Client.StartsWith("B") && dEntryDate > dProgramStartDate) // BCBSMA
                    {
                        exclude = "345";
                        ShowQuestions = true;
                    }
                    // figure out what scenario we have from the dates passed in
                    //if (dEntryDate > dProgramStartDatePlus180) // scenario 1 & 2
                    //{
                    //    if (dDateOfService < dProgramStartDate) // hard stop
                    //        echo = "Hard Stop" + Environment.NewLine;
                    //    else if (dDateOfService >= dProgramStartDate)
                    //    {
                    //        echo = "A12345" + Environment.NewLine;
                    //        exclude = "B";
                    //        ShowQuestions = true;
                    //    }
                    //    else
                    //        echo = "N/A" + Environment.NewLine;
                    //}
                    //else if (dEntryDate <= dProgramStartDatePlus180) // scenario 3, 4 & 5
                    //{
                    //    if (dDateOfService < dProgramStartDate) //scenario 3
                    //    {
                    //        echo = "B12345" + Environment.NewLine;
                    //        exclude = "A";
                    //        ShowQuestions = true;
                    //    }
                    //    else if (dDateOfService >= dProgramStartDate &&
                    //        dDateOfService <= dProgramStartDatePlus180) //scenario 4 (inside client administrative period)
                    //    {
                    //        echo = "12345" + Environment.NewLine;
                    //        exclude = "AB";
                    //        ShowQuestions = true;
                    //    }
                    //    else if (dDateOfService >= dProgramStartDatePlus180) //scenario 5 (outside client administrative period)
                    //    {
                    //        echo = "A12345" + Environment.NewLine;
                    //        exclude = "B";
                    //        ShowQuestions = true;
                    //    }
                    //    else
                    //        echo = "N/A" + Environment.NewLine;
                    //}
                    // if we need questions load em and remove our excluded questions
                }
                if (ShowQuestions)
                {
                    // get our full list
                    lQuestions = GetQuestionsList().ToList();
                    // remove our excludes
                    foreach (Char c in exclude)
                    { lQuestions.Remove(lQuestions.First(x => x.QuexId == c.ToString())); }
                    // do this just for logging the sequence
                    foreach (Question q in lQuestions)
                    { echo += q.QuexId; }
                }

            }
            catch (Exception ex) { echo += ex.Message; }
            return lQuestions;
        }

        // POST: api/CoC
        [HttpPost]
        [Route("api/coc")]
        public CoCResult Post([FromBody] CocRequest value)
        {
            // string json = JsonConvert.SerializeObject(value.AnswerList);
            string json = JsonConvert.SerializeObject(value);
            Debug.WriteLine(json);
            string jsonformatted = JValue.Parse(json).ToString(Formatting.Indented);
            Debug.WriteLine(jsonformatted);
            // var alist = JsonConvert.DeserializeObject<List<Question>>(json);
            var obj = JsonConvert.DeserializeObject<CocRequest>(json);
            var alist = obj.AnswerList;
            //Debug.WriteLine("num - " + obj.SomeNum);
            //Debug.WriteLine("str - " + obj.SomeStr);
            int one = 0;
            int two = 0;
            int three = 0;
            CoCResult result = new CoCResult();
            result.LevelOne = "Not Met";
            result.LevelTwo = "Not Met";
            result.LevelThree = "Not Met";
            result.CoCDetermination = "Not Met";
            result.BadgeFlag = false;

            foreach (Question a in alist)
            {
                if (a.Level == "1")
                {
                    one++;
                }
                if (a.Level == "2")
                {
                    two++;
                }
                if (a.Level == "3")
                {
                    three++;
                }
            }

            result.LevelOne = "Not Met";
            result.LevelTwo = "Not Met";
            result.CoCDetermination = "Not Met";
            if (obj.TreatmentType == 0)
                result.CoCDetermination = "N/A";
            else if (obj.TreatmentType == 1 && one > 1 && two > 0)
            {
                result.LevelOne = "Met";
                result.CoCDetermination = "Clinical";
            }
            else if (obj.TreatmentType == 2 && one > 1)
            {
                result.LevelOne = "Met";
                result.LevelTwo = "Met";
                result.CoCDetermination = "Admin";
            }

            if (!result.CoCDetermination.StartsWith("N"))
                result.BadgeFlag = true;

            result.Extra = "";
            return result;

        }

        // PUT: api/CoC/5
        [HttpPut]
        [Route("api/coc/{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete]
        [Route("api/coc/{id}")]
        public void Delete(int id)
        {
        }

        private IEnumerable<Question> GetQuestionsList()
        {
            string json = System.IO.File.ReadAllText(@"content\attestationsV2.json");
            var qlist = JsonConvert.DeserializeObject<List<Question>>(json);

            return qlist;
        }


    }
}
