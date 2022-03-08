using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using website.Models;

namespace website.Controllers
{
    public class recipesController : Controller
    {
        private Models.tarifsitesiEntities1 db = new Models.tarifsitesiEntities1();

        // GET: recipes


        //public ActionResult Index()
        //{
        //    var recipes = db.recipes.Include(r => r.categories).Include(r => r.users);
        //    return View(recipes.ToList());
        //}

        public ActionResult Index(string aranacakKelime)
        {
            var yemekler= db.recipes.Include(r => r.categories).Include(r => r.users);
           
            return View(yemekler.ToList());
        }

        public ActionResult YemekAra(string Ara = null, string[] Employee = null)
        {
            int k = 0;
            int j = 0;

            var aranan = db.recipes.ToList();
            if(Ara!="")
            {
                aranan = db.recipes.Where(m => m.name.Contains(Ara)).ToList();
                var sql = db.Database.SqlQuery<comments>("Select * from comments").Where(i => i.meal_id == i.meal_id).ToList();
            }
            

            else if(Employee.Count()!=0)
            {
                for(k=0;k<Employee.Count();k++)
                {
                    string ara = Employee[k];
                    aranan = db.recipes.Where(m => m.material.Contains(ara)).ToList();
                    if(aranan.Count()==0)
                    {
                        break;
                    }

                    else
                    {
                        j += 1;
                    }
                }

                if(k==j)
                {
                    string ara1 = Employee[0];
                    string ara2 = Employee[1];
                    aranan = db.recipes.Where(m => m.material.Contains(ara1) 
                    && m.material.Contains(ara2)).ToList();
                    var sql = db.Database.SqlQuery<comments>("Select * from comments").Where(i => i.meal_id == i.meal_id).ToList();
                }
                
                
            }
            

            return View(aranan.OrderByDescending(m=>m.date));
        }

        // GET: recipes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            recipes recipes = db.recipes.Find(id);
            if (recipes == null)
            {
                return HttpNotFound();
            }
            users users = db.users.Find(recipes.user_id );
            ViewBag.name = users.name_sname;
            Session["uye_id"] = users.id;
            ViewBag.user_id = new SelectList(db.users, "id", "name_sname", recipes.user_id);
            ViewBag.Query = db.recipes.Find(id);
            DateTime baslamaTarihi = new DateTime(2014, 10, 01);
            DateTime bitisTarihi = new DateTime(2015, 12, 15);

            TimeSpan kalangun = bitisTarihi - baslamaTarihi;//Sonucu zaman olarak döndürür
            int toplamGun = Convert.ToInt32( kalangun.TotalDays);
            int year = toplamGun / 365;
            int month = (toplamGun%365)/30;
            int day = (toplamGun % 365)%30;
            ViewBag.year = year;
            ViewBag.month = month;
            ViewBag.day = day;
            return View(recipes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(string yorum, int? id)
        {
            id = 9;
            recipes tarifler = db.recipes.Find(id);
            try
            {
                comments comments = new comments();
                comments.meal_id = Convert.ToInt32(id);
                comments.user_id = Convert.ToInt32(Session["uye_id"]);
                comments.confirmation = true;
                comments.content = yorum;
                comments.date = DateTime.Now;
                db.comments.Add(comments);
                db.SaveChanges();
                TempData["alert"] = "<script>alert('Yorumunuz kaydedildi');</script>";
                
            }

            catch(Exception)
            {
                TempData["alert"] = "<script>alert('Hata Oluştu');</script>";
            }

            ViewBag.Query = db.recipes.Find(id);
            return RedirectToAction("Details", new { id = id});
        }

        [HttpPost]
        public JsonResult Yorum(string yorum, int id)
        {
            var userid = Session["uye_id"];
            if (yorum == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            db.comments.Add(new comments { user_id = Convert.ToInt32(userid), meal_id = id, content = yorum, date = DateTime.Now, confirmation = true });
            db.SaveChanges();
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // GET: recipes/Create
        public ActionResult Create()
        {
            ViewBag.categori_id = new SelectList(db.categories, "id", "name");
            ViewBag.user_id = new SelectList(db.users, "id", "name_sname");
            return View();
        }

        // POST: recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(recipes recipes)
        {
            //if (ResimDosya != null)
            //{
            //    foreach (var item in ResimDosya)
            //    {
            //        item.SaveAs(Server.MapPath("~/Content/img/" + item.FileName));
            //        recipes.image = item.FileName;
            //    }
            //}
            if (ModelState.IsValid)
            {
                var userid = Session["uye_id"];
                recipes.user_id = Convert.ToInt32(userid);
                recipes.date = DateTime.Now;
                recipes.comments = null;
                db.recipes.Add(recipes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.categori_id = new SelectList(db.categories, "id", "name", recipes.categori_id);
            ViewBag.user_id = new SelectList(db.users, "id", "name_sname", recipes.user_id);
            return View(recipes);
        }

        // GET: recipes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            recipes recipes = db.recipes.Find(id);
            if (recipes == null)
            {
                return HttpNotFound();
            }
            ViewBag.categori_id = new SelectList(db.categories, "id", "name", recipes.categori_id);
            ViewBag.user_id = new SelectList(db.users, "id", "name_sname", recipes.user_id);
            return View(recipes);
        }

        // POST: recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,material,praparation,image,date,point,categori_id,user_id,video")] recipes recipes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.categori_id = new SelectList(db.categories, "id", "name", recipes.categori_id);
            ViewBag.user_id = new SelectList(db.users, "id", "name_sname", recipes.user_id);
            return View(recipes);
        }

        // GET: recipes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            recipes recipes = db.recipes.Find(id);
            if (recipes == null)
            {
                return HttpNotFound();
            }
            return View(recipes);
        }

        // POST: recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            recipes recipes = db.recipes.Find(id);
            db.recipes.Remove(recipes);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
