using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using website.Models;

namespace website.Controllers
{
    public class HomeController : Controller
    {
        private Models.tarifsitesiEntities1 db = new tarifsitesiEntities1();
        private Models.Class1 veriler = new Models.Class1();
        
        public ActionResult Index()
        {
            //recipes model = new recipes();
            //model.malzemelist = new List<recipes>();
            ////Çalışanlarımızı listemize aktarıyorum  
            //model.malzemelist = CountryDate();
            //ViewBag.Employees = Employees;
            return View();
        }
        
        

        public List<recipes> CountryDate()
        {
            List<recipes> objcountry = new List<recipes>();
            objcountry.Add(new recipes { name = "America" });
            objcountry.Add(new recipes { name = "India" });
            objcountry.Add(new recipes { name = "Sri Lanka" });
            objcountry.Add(new recipes { name = "China" });
            objcountry.Add(new recipes { name = "Pakistan" });
            return objcountry;
        }


        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register([Bind(Include = "id,name_sname,mail,password,gender")] users users)
        {
            if (ModelState.IsValid)
            {
                db.users.Add(users);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(users);
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(users model)
        {
            users users = new users();
            var u = db.users.FirstOrDefault(x => x.mail == model.mail && x.password == model.password);
            if (u!=null)
            {
                FormsAuthentication.SetAuthCookie(u.name_sname, false);
                Session["user"] = u.name_sname;
                Session["uye_id"] = u.id;
                Session["img"] = u.image;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.LoginError = "Hatalı kullanıcı adı veya şifre";
            }

            ViewBag.user = Session["uye_id"];
            return View();
        }

        public ActionResult Logout()
        {
            DateTime now = DateTime.UtcNow;
            var cookie = new HttpCookie(User.Identity.Name, null)
            {
                Expires = now.AddDays(-1)
            };
            HttpContext.Response.Cookies.Set(cookie);
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }

        public JsonResult SendRating(string r, string s, string id, string url)
        {
            int autoId = 0;
            Int16 thisVote = 0;
            Int16 sectionId = 0;
            Int16.TryParse(s, out sectionId);
            Int16.TryParse(r, out thisVote);
            int.TryParse(id, out autoId);

            if (!User.Identity.IsAuthenticated)
            {
                return Json("Kullanıcı girişi yapılmamış!");
            }

            if (autoId.Equals(0))
            {
                return Json("<br/>Maalesef oy kullanma kaydı mevcut değil");
            }

            switch (s)
            {
                case "5": // school voting
                    // check if he has already voted
                    var isIt = db.VoteLog.Where(v => v.SectionId == sectionId 
                        && v.VoteForId == autoId && v.UserName == User.Identity.Name).FirstOrDefault();
                    if (isIt != null)
                    {
                        // keep the school voting flag to stop voting by this member
                        HttpCookie cookie = new HttpCookie(User.Identity.Name, "true");
                        Response.Cookies.Add(cookie);
                        return Json("Zaten bu tarifi oyladınız. Teşekkürler !");
                    }

                    var sch = db.recipes.Where(sc => sc.id == autoId).FirstOrDefault();
                    if (sch != null)
                    {
                        object obj = sch.Vote;

                        string updatedVotes = string.Empty;
                        string[] votes = null;
                        if (obj != null && obj.ToString().Length > 0)
                        {
                            string currentVotes = obj.ToString(); // votes pattern will be 0,0,0,0,0
                            votes = currentVotes.Split(',');
                            // if proper vote data is there in the database
                            if (votes.Length.Equals(5))
                            {
                                // get the current number of vote count of the selected vote, always say -1 than the current vote in the array 
                                int currentNumberOfVote = int.Parse(votes[thisVote - 1]);
                                // increase 1 for this vote
                                currentNumberOfVote++;
                                // set the updated value into the selected votes
                                votes[thisVote - 1] = currentNumberOfVote.ToString();
                            }
                            else
                            {
                                votes = new string[] { "0", "0", "0", "0", "0" };
                                votes[thisVote - 1] = "1";
                            }
                        }
                        else
                        {
                            votes = new string[] { "0", "0", "0", "0", "0" };
                            votes[thisVote - 1] = "1";
                        }

                        // concatenate all arrays now
                        foreach (string ss in votes)
                        {
                            updatedVotes += ss + ",";
                        }
                        updatedVotes = updatedVotes.Substring(0, updatedVotes.Length - 1);

                        db.Entry(sch).State = EntityState.Modified;
                        sch.Vote = updatedVotes;
                        db.SaveChanges();

                        VoteLog vm = new VoteLog()
                        {
                            Active = true,
                            SectionId = Int16.Parse(s),
                            UserName = User.Identity.Name,
                            Vote = thisVote,
                            VoteForId = autoId
                        };

                        db.VoteLog.Add(vm);

                        db.SaveChanges();

                        // keep the school voting flag to stop voting by this member
                        HttpCookie cookie = new HttpCookie(User.Identity.Name, "true");
                        Response.Cookies.Add(cookie);
                        
                    }
                    break;
                default:
                    break;
            }
            return Json("<br />Bu tarife " + r + " yıldız verdiniz, teşekkürler !");
        }

        public ActionResult KategoriYemek(int? id)
        {
            var yemekler = db.recipes.Where(m => m.categori_id == id).ToList();
            return View(yemekler);
        }


        public PartialViewResult Kategoriler(int? id)
        {
            var maincats = db.categories.Where(m => m.parrent_id == 0).ToList();
            var subcats = db.categories.Where(m => m.parrent_id == id).ToList();
            Class1 models = new Class1();
            List<categories> main = db.categories.ToList();
            List<categories> sub = db.categories.ToList();
            if (subcats.Count == 0)
            {
                maincats = db.categories.Where(m => m.parrent_id == 0).ToList();
                subcats = db.categories.Where(m => m.id == 0).ToList();
            }

            else
            {
                maincats = db.categories.Where(m => m.id == id).ToList();
                subcats = db.categories.Where(m => m.parrent_id == id).ToList();
            }
            main = maincats;
            sub = subcats;
            models.Tub = new Tuple<List<categories>, List<categories>>(main, sub);
            return PartialView(models);
        }
        
    }
}