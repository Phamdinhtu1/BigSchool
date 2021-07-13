using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BigSchool.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BigSchool.Controllers
{
    public class CoursesController : Controller
    {
        private BigSchoolContext db = new BigSchoolContext();

        // GET: Courses
        public ActionResult Index()
        {
            return View(db.Courses.ToList());
        }


        // GET: Courses/Create
        public ActionResult Create()
        {
            BigSchoolContext context = new BigSchoolContext();
            Course objCourse = new Course();
            objCourse.ListCategory = context.Categories.ToList();
            return View(objCourse);

        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course course)
        {
            BigSchoolContext context = new BigSchoolContext();
            if (ModelState.IsValid)
            {
                course.ListCategory = context.Categories.ToList();
                return View("Create",course);
            }


            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            course.LectureId = user.Id;

            db.Courses.Add(course);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }
        public ActionResult Mine()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAtten = context.Attendances.Where(m => m.Attendee == user.Id).ToList();
            List<Course> courses = context.Courses.Where(c => c.DateTime > DateTime.Now).ToList();
            var course1 = new List<Course>();
            foreach (Attendance tmp in listAtten)
            {
                foreach  (Course c in courses)
                {
                    if (tmp.CourseId==c.Id)
                    {
                        course1.Add(c);
                    }
                }
            }
            return View(course1);
        }
        public ActionResult Attending()
        {
            BigSchoolContext context = new BigSchoolContext();
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            var listAtten = context.Attendances.Where(m => m.Attendee == user.Id).ToList();
            var courses = new List<Course>();
            foreach (Attendance tmp in listAtten)
            {
                Course obj = tmp.Course;
                courses.Add(obj);
            }
            return View(courses);
        }

        // POST: Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,LectureId,Place,DateTime,CategoryId")] Course course)
        //{
        //        if (ModelState.IsValid)
        //        {
        //            db.Courses.Add(course);
        //            db.SaveChanges();
        //            return RedirectToAction("Index");
        //}

        //        return View(course);
        //}

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser currentUser = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId()); var userID = User.Identity.GetUserId();
            BigSchoolContext context = new BigSchoolContext();
            Course courses = db.Courses.Find(id);
            courses.ListCategory = context.Categories.ToList();
            //var courses = context.Courses.Where(c => c.LectureId == currentUser.Id && c.DateTime > DateTime.Now).ToList();
            //List<Course> courses = context.Courses.Where(c => c.LectureId == currentUser.Id && c.DateTime > DateTime.Now).ToList();
            if (courses == null)
            {
                return HttpNotFound();
            }
            return View(courses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,LectureId,Place,DateTime,CategoryId")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(course);
        }

        //// GET: Courses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            ApplicationUser user = System.Web.HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>().FindById(System.Web.HttpContext.Current.User.Identity.GetUserId());
            Attendance attendance = db.Attendances.Find(id,user.Id);
            db.Attendances.Remove(attendance);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        //// GET: Courses/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Course course = db.Courses.Find(id);
        //    if (course == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(course);
        //}
        //// POST: Courses/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
    }
}
