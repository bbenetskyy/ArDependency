﻿
using DataParser.Enums;
using DataParser.Extensions;
using DataParser.Models;
using FormulasCollection.Models;
using NLog;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataParser.DefaultRealization
{
    public class MarathonParser
    {
        #region [Public field]
        public List<ResultForForks> result;
        #endregion

        #region [Private field]
        private Dictionary<string, EnglishNameTeams> englishNameTeams_Dictionary;

        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private PhantomJSDriver _driver;
        OpenQA.Selenium.Firefox.FirefoxDriver firefox;
        private string isClick_IdEvent = ".";
        private string RefreshPage = "";
        #endregion

        #region[Static field]
        public static List<ResultForForks> winik = new List<ResultForForks>();
        #endregion

        #region [Constructors]
        public MarathonParser()
        {
            #region Selenium
            /*
                        var prof = new FirefoxProfile();
                        prof.SetPreference("browser.startup.homepage_override.mstone",
                            "ignore");
                        prof.SetPreference("startup.homepage_welcome_url.additional",
                            "about:blank");
                        prof.EnableNativeEvents = false;

                         firefox = new OpenQA.Selenium.Firefox.FirefoxDriver(prof);
                         firefox.Manage().Window.Maximize();
                         firefox.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(35));
                         firefox.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromMinutes(35));
                         firefox.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromMinutes(35));*/

            /*
             var driverService = PhantomJSDriverService.CreateDefaultService(); 
             _driver = new PhantomJSDriver();
             _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromMinutes(35));
             _driver.Manage().Timeouts().SetPageLoadTimeout(TimeSpan.FromMinutes(35));
             _driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromMinutes(35));*/
            #endregion

            result = new List<ResultForForks>();
            // Initi(sportType);
        }
        #endregion

        #region [Initi]
        public async Task<List<ResultForForks>> InitiAsync(SportType sportType)
        {

            this.englishNameTeams_Dictionary = await this.GetEnglishNameTEams(sportType).ConfigureAwait(false);
            try
            {
                var result = await GetNameTeamsAndDateAsync(sportType).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                var ee = e.StackTrace;
                var eee = e.Message;
            }
            return result;

            return new List<ResultForForks>();
            //I guess it must be cause when Exception is not catch(Exception ex)ed, it will show Exception in program,
            // so in my opinion it will be better to return zero Forks from this type one
        }
        #endregion

        #region [Parse]
        private async Task<List<ResultForForks>> GetNameTeamsAndDateAsync(SportType sportType)
        {
            result.Clear();

            ResultForForks teamToAdd = null;

            //List<string>
            List<string> list_coeffType = new List<string>();

            //integer
            int indexEvent = 0;

            //string
            string url = string.Empty;
            string namefile = string.Empty;
            //string selectedEvent = string.Empty;
            string eventid = string.Empty;
            //string totalOrFora = string.Empty;

            //bool
            bool isTypeCoff = false;
            bool canParseDataToTeam = false;
            bool isEventID = false;
            //bool isTeamName = false;
            bool isDate = false;
            bool isDataToAutoPlay = false;
            bool isSelectionKey = false;
            bool isValueCoef = false;
            bool isFora = false;
            bool isTotal = false;

            //bool isChangeEvent = false;

            bool isLigue = false;

            DataMarathonForAutoPlays obj = new DataMarathonForAutoPlays();



            UrlAndNameFile(sportType, out url, out namefile);
            string gotHtml = await HtmlAsync(url).ConfigureAwait(false);
            string[] lines = gotHtml.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                try
                {
                    if (lines[i].Contains(">Итоги<") || lines[i].Contains(">Итоги.<"))
                        break;
                    if (lines[i].Contains(MarathonTags.newTypeCoeff))
                    {
                        isTypeCoff = true;
                        indexEvent = 0;
                        list_coeffType = new List<string>();
                    }
                    else if (isTypeCoff && lines[i].Contains(MarathonTags.newTag_TypeCoeff))
                    {
                        isTypeCoff = false;
                    }
                    if (lines[i]._Contains(MarathonTags.mainTagForEvent, MarathonTags.newEventID))
                    {
                        canParseDataToTeam = true;
                        isEventID = true;
                    }
                    /*else if (lines[i].Contains(MarathonTags.newTeamName))
                    {
                        isTeamName = true;
                    }*/
                    else if (lines[i].Contains(MarathonTags.newDate))
                    {
                        isDate = true;
                    }
                    else if (lines[i].Contains(MarathonTags.newAutoplay))
                    {
                        isDataToAutoPlay = true;
                    }
                    else if (lines[i].Contains(MarathonTags.newSelection_key))
                    {
                        isSelectionKey = true;
                    }
                    else if (lines[i].Contains(MarathonTags.newValueCoef))
                    {
                        isValueCoef = true;
                    }
                    else if (lines[i].Contains(MarathonTags.newFora))
                    {
                        isFora = true;
                    }
                    else if (lines[i].Contains(MarathonTags.newTotal))
                    {
                        isTotal = true;
                    }


                    //Parse
                    if (isTypeCoff)
                    {
                        if (lines[i].Contains(MarathonTags.newclassToTypeCoef))
                        {
                            i++;
                            string typeCoeff = string.Empty;
                            if (lines[i].Contains("<b>"))
                            {
                                string start = lines[i].getValueWithoutStartTags().Trim();
                                start = start.Replace(" ", "");
                                string newLine = !string.IsNullOrEmpty(start) ? lines[i].Replace(start, "") : lines[i];
                                start = start.Equals("Фора") ? "F" : start;
                                //start = start.Equals("Тотал") ? "T" : start;
                                typeCoeff = newLine.GetAttribut();
                                typeCoeff = start + typeCoeff;
                            }
                            else
                            {
                                typeCoeff = lines[i].Contains("Меньше") ? "TU" : (lines[i].Contains("Больше") ? "TO" : "NOForaNOTotal");
                                
                            }
                            typeCoeff = typeCoeff.Replace(" ", "");
                            if (!string.IsNullOrEmpty(typeCoeff))
                            {
                                if (list_coeffType.Equals(typeCoeff))
                                {
                                    isTypeCoff = false;
                                }
                                else {
                                    list_coeffType.Add(typeCoeff);
                                }
                            }
                        }
                    }
                    

                    if (isEventID)
                    {
                        string eventID = lines[i].GetEventID();
                        if (!eventID.Equals(eventid))
                        {
                            teamToAdd = new ResultForForks();
                            indexEvent = 0;
                        }
                        teamToAdd.EventId = eventID;
                        indexEvent = 0;
                        isTypeCoff = false;
                        isEventID = false;
                        eventid = eventID;

                    }

                    /*if (isTeamName)
                    {
                        
                        if (string.IsNullOrEmpty(teamToAdd.Event))
                        {
                            var teamName = lines[i].GetAttribut();
                            //teamToAdd.Event = teamName.Trim(' ');
                        }
                        else
                        {
                            i++;
                            var teamName = lines[i].GetAttribut();
                            //teamToAdd.Event += " - " + teamName.Trim(' ');
                            isTeamName = false;
                        }
                    }*/
                    if (isDate)
                    {
                        i++;
                        teamToAdd.MatchDateTime = lines[i].Replace(" ", "");
                        isDate = false;
                    }
                    if (isDataToAutoPlay || isSelectionKey)
                    {
                        if (lines[i].Contains(Tags_DataMarathonForAutoPlays.data_sel))
                        {
                            obj = ParseForAutoPlay(lines[i], Tags_DataMarathonForAutoPlays.data_sel);
                            isDataToAutoPlay = false;
                        }
                        else if (lines[i].Contains(Tags_DataMarathonForAutoPlays.data_selection_key))
                        {
                            obj = ParseForAutoPlay(lines[i], Tags_DataMarathonForAutoPlays.data_selection_key, obj);
                            teamToAdd.marathonAutoPlay = obj;
                            isSelectionKey = false;
                        }

                    }
                    if (isValueCoef)
                    {
                        var valueCoeff = lines[i].GetAttribut2();
                        teamToAdd.Coef = valueCoeff;
                        isValueCoef = false;
                        if (!isFora && !isTotal && string.IsNullOrEmpty(teamToAdd.Type))
                        {
                            indexEvent = (indexEvent >= list_coeffType.Count) ? 0 : indexEvent;
                            var typecoeff = list_coeffType[indexEvent];
                            teamToAdd.Type = typecoeff;
                            indexEvent++;
                        }
                        isValueCoef = false;
                    }
                    if (isFora || isTotal)
                    {
                        i++;
                        indexEvent = (indexEvent >= list_coeffType.Count) ? 0 : indexEvent;
                        string totalOrFora = lines[i].getValueWithoutStartTags();
                        var typecoeff = list_coeffType[indexEvent] + totalOrFora.Replace(" ","");
                        teamToAdd.Type = typecoeff;
                        indexEvent++;
                        isFora = false;
                        isTotal = false;
                    }

                    if (teamToAdd != null)
                    {
                        if (teamToAdd.isFullData())
                        {
                            teamToAdd.League = "NON";
                            teamToAdd.SportType = sportType.ToString();
                            teamToAdd.Bookmaker = Site.MarathonBet.ToString();
                            if (englishNameTeams_Dictionary.ContainsKey(teamToAdd.EventId))
                            {
                                teamToAdd.Event = englishNameTeams_Dictionary[teamToAdd.EventId].name1 + " - " + englishNameTeams_Dictionary[teamToAdd.EventId].name2;
                            }
                            else
                            {
                                MessageBox.Show("Event not found!!!    EventID :  " + teamToAdd.EventId);
                            }
                            var newteamToAdd = teamToAdd;
                            result.Add(new ResultForForks() {
                                Bookmaker = newteamToAdd.Bookmaker,
                                Type = newteamToAdd.Type,
                                Coef = newteamToAdd.Coef,
                                Event = newteamToAdd.Event,
                                EventId = newteamToAdd.EventId,
                                League = newteamToAdd.League,
                                MatchDateTime = newteamToAdd.MatchDateTime,
                                marathonAutoPlay = newteamToAdd.marathonAutoPlay,
                                SportType = newteamToAdd.SportType});

                            teamToAdd.Coef = string.Empty;
                            teamToAdd.Type = string.Empty;
                            teamToAdd.marathonAutoPlay = null;

                            isTypeCoff = false;
                            canParseDataToTeam = false;
                            isEventID = false;
                            //isTeamName = false;
                            isDate = false;
                            isDataToAutoPlay = false;
                            isSelectionKey = false;
                            isValueCoef = false;
                            isFora = false;
                        }
                    }
                }
                catch (Exception e)
                {
                    /* if (!File.Exists("log.txt"))
                     {
                         File.Create("log.txt");
                     }
                     using (StreamWriter sw = new StreamWriter("log.txt", true))
                     {
                         sw.WriteLine("EXCEPTION: " + teamToAdd.EventToString());
                         sw.Close();
                     }*/
                    MessageBox.Show(e.StackTrace + "/n" + e.Message);
                }
            }
            return result;
        }

        private async Task<Dictionary<string, EnglishNameTeams>> GetEnglishNameTEams(SportType sportType)
        {
            var resultEnglishTeams = new Dictionary<string, EnglishNameTeams>();
            var url = "";
            var namefile = "";
            string name1 = null;
            string name2 = null;
            string _eventid = null;


            UrlAndNameFile(sportType, out url, out namefile, true);
            var lines = (await HtmlAsync(url, false).ConfigureAwait(false)).Split('\n');


            foreach (var line in lines)
            {
                if (line._Contains(MarathonTags.EventID))
                    _eventid = line.GetEventID();

                if (line._Contains(MarathonTags.NameTeam))
                {
                    if (name1 == null)
                        name1 = GetAttribut(line);// line.Substrings(Tags.NameTeam);
                    else name2 = GetAttribut(line);//line.Substrings(Tags.NameTeam);
                }
                if (!string.IsNullOrEmpty(name1) && !string.IsNullOrEmpty(name2) && !string.IsNullOrEmpty(_eventid))
                {
                    var teams = new EnglishNameTeams();
                    teams.name1 = name1;
                    teams.name2 = name2;
                    resultEnglishTeams.Add(_eventid, teams);
                    _eventid = null;
                    name1 = null;
                    name2 = null;
                }
            }


            return resultEnglishTeams;
        }

        private DataMarathonForAutoPlays ParseForAutoPlay(string line, string tag, DataMarathonForAutoPlays obj = null)
        {

            obj = obj == null ? (obj = new DataMarathonForAutoPlays()) : obj;
            if (tag.Equals(Tags_DataMarathonForAutoPlays.data_sel))
            {
                line = line.TagsContent2(Tags_DataMarathonForAutoPlays.data_sel);
                var a = getString(line);

                var b = a.Split(':');

                var b1 = b[0].Replace("[", "").Replace("]", "").Split(',');
                var price = b[1].Replace("[", "").Replace("]", "").Split(',');

                var subj = line.Split(',');
                obj.sn = b1[0];
                obj.mn = b1[1];
                obj.ewc = b1[2];
                obj.cid = b1[3];
                obj.prt = b1[4];
                obj.ewf = b1[5];
                obj.epr = b1[6];

                for (int i = 0; i < price.Length; i++)
                {
                    obj.prices.Add(price[i]);
                }
            }
            if (tag.Equals(Tags_DataMarathonForAutoPlays.data_selection_key))
            {
                obj.selection_key = line.TagsContent(Tags_DataMarathonForAutoPlays.data_selection_key);
            }
            return obj;
        }
        /*{"sn":"Burgos (+9.5)",
  "mn":"Победа с учетом форы",
  "ewc":"1/1 1",
  "cid":10182298350,
  "prt":"CP",
  "ewf":"1.0",
  "epr":"1.83",
  "prices"
      :{"0":"83/100",
        "1":"1.83",
        "2":"-121",
        "3":"0.83",
        "4":"0.83",
        "5":"-1.21"}}*/
        private string getString(string line)
        {
            line = line.Trim('{');
            string result = "";
            bool find_ = false; // :
            bool _find = false; // {
            string element = "";
            string price = "";
            foreach (var l in line)
            {

                if (l == ',')
                {

                    if (find_ && !_find)
                    {
                        find_ = false;
                        result += (!string.IsNullOrEmpty(result)) ? "," : "[";
                        result += element.Trim('\"');
                        element = "";
                    }
                    if (find_ && _find)
                    {
                        find_ = false;
                        price += (!string.IsNullOrEmpty(price)) ? "," : "[";
                        price += element.Trim('\"');
                        element = "";
                    }

                }
                if (l == '{' || l == '}')
                {
                    find_ = false;
                }
                if (find_)
                {

                    element += l;

                }
                if (l == ':')
                {
                    find_ = true;
                }
                if (l == '{')
                {
                    _find = true;
                }

            }
            result += "]:";
            result += (price + "," + element + "]");

            return result;
        }
        #endregion

        #region[Page and URL]
        private static async Task<string> HtmlAsync(string url, bool a = true)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);
            List<string> result = new List<string>();
            string HTML = null;
            StreamReader reader = null;
            try
            {
                Debug.Assert(response != null, "response != null");
                reader = new StreamReader(response.GetResponseStream());
                HTML = await reader.ReadToEndAsync().ConfigureAwait(false);
                reader.Close();
            }
            catch (FileLoadException ex)
            {
                _logger.Error(ex.Message); _logger.Error(ex.StackTrace);
                //reader.Close();
            }
            finally
            {
                reader?.Close();
            }
            WriteToDocument(HTML);
            return HTML;
        }
        private void UrlAndNameFile(SportType sportType, out string url, out string namefile, bool isEnglish = false)
        {
            string language = isEnglish ? "en" : "su";
            string en_namefile = isEnglish ? "en" : "";
            url = "https://www.marathonbet.com/" + language + "/betting/Ice+Hockey";
            namefile = "Default.html";
            switch (sportType)
            {
                case SportType.Soccer:
                    namefile = "Soccer" + en_namefile + ".html";
                    url = "https://www.marathonbet.com/" + language + "/betting/Football/"; //"/betting/Football/England/Championship/Promotion+Play-Offs/Semi+Final/1st+Leg/";
                    break;

                case SportType.Basketball:
                    namefile = "Basketball" + en_namefile + ".html";
                    url = "https://www.marathonbet.com/" + language + "/betting/Basketball/";
                    break;

                case SportType.Hockey:
                    namefile = "Hokey" + en_namefile + ".html";
                    url = "https://www.marathonbet.com/" + language + "/betting/Ice+Hockey/";
                    break;

                case SportType.Tennis:
                    namefile = "Tenis" + en_namefile + ".html";
                    url = "https://www.marathonbet.com/" + language + "/betting/Tennis/";
                    break;

                case SportType.Volleyball:
                    namefile = "Volleyball" + en_namefile + ".html";
                    url = "https://www.marathonbet.com/" + language + "/betting/Volleyball/";
                    break;
            }
        }
        #endregion

        #region [Documents]
        public static void WriteToDocument(List<ResultForForks> teams, string namefile = "check.txt")
        {
            if (File.Exists(namefile))
                File.Delete(namefile);
            //File.Create(namefile);
            StringBuilder sb = new StringBuilder();
            //StreamWriter sw = new StreamWriter(namefile, true);

            foreach (var team in teams)
            {
                sb.AppendLine(team.EventToString());
            }
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine();


            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                var a = sb.ToString().Split('\n');
                var b = sb.ToString();
                //sw.WriteLineAsync(b);
                // File.WriteAllLines(namefile, a);

                StreamWriter writer = null;
                writer = new StreamWriter(namefile);
                writer.WriteLine(b);
                writer.Close();


            }
        }

        private static void WriteToDocument(string html)
        {
            using (StreamWriter sw = new StreamWriter("html__.txt"))
            {
                sw.WriteLine(html);
                sw.Close();
            }
        }
        #endregion

        #region [Help]
        //------------------HELP_PARSE------------------------------
        private static string GetAttribut(string line, bool date = false)
        {
            bool isStartTag = true;
            bool isFinish = false;
            string result = "";
            foreach (var l in line)
            {
                if (l == '<')
                    isStartTag = false;
                if (isStartTag)
                    result += l;
                if (l == '>')
                    isStartTag = true;
                if (String.IsNullOrEmpty(result.Trim()) && !isStartTag)
                    result = "";
                if (!String.IsNullOrEmpty(result.Trim()) && !isStartTag)
                {
                    isFinish = true;
                    result += (date && !result.Contains("/2016")) ? "/2016 " : "";

                }
                if (isFinish && !date)
                    return result;
            }
            return !date ? "" : result;
        }
        private static string GetLigue(string line)
        {
            bool isStartTag = true;
            string result = "";
            foreach (var l in line)
            {
                if (l == '<')
                    isStartTag = false;
                if (isStartTag)
                    result += l;
                if (l == '>')
                    isStartTag = true;
                if (String.IsNullOrEmpty(result.Trim()) && !isStartTag)
                    result = "";
            }
            return result;
        }
        #endregion

        #region [Selenium]
        private string PhantomDriver(string urlTypeSport, string id, string ligaID, bool today, SportType sporttype)
        {
            id = id.Contains("event_") ? id : "event_" + id;
            string liveTag = today ? "today-name" : "name";
            var res = "";

            try
            {
                if (!String.IsNullOrEmpty(this.isClick_IdEvent))
                {
                    if (this.isClick_IdEvent != id)
                    {
                        _driver.Navigate().GoToUrl(urlTypeSport);
                        var idTag = _driver.FindElement(By.Id(id));
                        var click = idTag.FindElement(By.ClassName(liveTag));
                        click.Click();

                        //var a = _driver.FindElement(By.Id(id)).FindElement(By.ClassName("blocks-area"));
                        var a = _driver.FindElement(By.Id(id));
                        res = a.Text;
                    }
                }
            }
            catch (Exception e)
            {
                /*MessageBox.Show(e.Message.ToString());
                _driver.Close();
                _driver = new PhantomJSDriver();*/
                //return PhantomDriver(urlTypeSport, id, ligaID, today, sporttype);
            }
            this.isClick_IdEvent = id;
            this.WriteToDocumentWithSelenium(res, sporttype, ligaID, id);
            return res;
        }
        private string PhantomFireFox(string urlTypeSport, string id, string ligaID, bool today, SportType sporttype)
        {
            id = id.Contains("event_") ? id : "event_" + id;
            string liveTag = today ? "today-name" : "name";
            var res = "";

            try
            {
                if (!String.IsNullOrEmpty(this.isClick_IdEvent))
                {
                    if (this.isClick_IdEvent != id)
                    {
                        firefox.Navigate().GoToUrl(urlTypeSport);
                        var idTag = firefox.FindElement(By.Id(id));
                        var click = idTag.FindElement(By.ClassName(liveTag));
                        click.Click();

                        //var a = firefox.FindElement(By.Id(id)).FindElement(By.ClassName("blocks-area"));
                        var a = firefox.FindElement(By.Id(id));
                        res = a.Text;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message.ToString());
                firefox.Close();
                firefox = new OpenQA.Selenium.Firefox.FirefoxDriver();
                return PhantomFireFox(urlTypeSport, id, ligaID, today, sporttype);
            }
            this.isClick_IdEvent = id;
            this.WriteToDocumentWithSelenium(res, sporttype, ligaID, id);
            return res;
        }
        private void WriteToDocumentWithSelenium(string data, SportType sporttype, string LigueId, string id)
        {
            string mainNameFolder = "Selenium Results";
            string nameFolderForTypeSport = sporttype.ToString();
            string nameFolderForLigueId = LigueId;
            string nameFile = id + ".txt";
            string path = "";
            if (!Directory.Exists(mainNameFolder))
            {
                Directory.CreateDirectory(mainNameFolder);
            }
            path += mainNameFolder + "\\";
            if (!Directory.Exists(path + nameFolderForTypeSport))
            {
                Directory.CreateDirectory(path + nameFolderForTypeSport);
            }
            path += nameFolderForTypeSport + "\\";
            if (!Directory.Exists(path + nameFolderForLigueId))
            {
                Directory.CreateDirectory(path + nameFolderForLigueId);
            }
            path += nameFolderForLigueId + "\\";
            if (!File.Exists(path + nameFile))
            {
                path += nameFile;
                using (StreamWriter sw = new StreamWriter(path))
                {
                    sw.WriteLine(data);
                    sw.Close();
                }
            }
        }
        #endregion

    }
}