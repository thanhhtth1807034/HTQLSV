using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HTQLSV.Models;
using PagedList;

namespace HTQLSV.Controllers
{
    public class StudentsController : Controller
    {
        private MyDbContext db = new MyDbContext();

        // GET: Students

        public ActionResult GetListStudentData()
        {
            return new JsonResult()
            {
                Data = db.Students.Where(s => s.Status != StudentStatus.DELETED).ToList(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult Index(string sortOrder, string searchKeyword, string currentFilter, int? page, int? limit, DateTime? start, DateTime? end)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.EmailSortParm = string.IsNullOrEmpty(sortOrder) ? "email_desc" : "";
            ViewBag.DateSortParm = sortOrder == "CreatedAt" ? "date_desc" : "CreatedAt";

            if (searchKeyword != null)
            {
                page = 1;
            }
            else
            {
                searchKeyword = currentFilter;
            }

            ViewBag.CurrentFilter = searchKeyword;

            var students = from s in db.Students.Where(s => s.Status != StudentStatus.DELETED) select s;
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                students = students.Where(s => s.FullName.Contains(searchKeyword) || s.Email.Contains(searchKeyword));
            }

            //if (fromDate != null && toDate != null)
            //{
            //    students = students.Where(c => c.CreatedAt >= fromDate && c.CreatedAt < toDate);
            //    ViewBag.currentFromDate = fromDate;
            //    ViewBag.currentToDate = toDate;
            //}
            var startTime = DateTime.Now;
            startTime = startTime.AddYears(-1);
            try
            {
                startTime = DateTime.Parse(start.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            var endTime = DateTime.Now;
            try
            {
                endTime = DateTime.Parse(end.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            students = students.Where(s => s.CreatedAt >= startTime && s.CreatedAt <= endTime);
            ViewBag.Start = startTime;
            ViewBag.End = endTime;

            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.FullName);
                    break;
                case "email_desc":
                    students = students.OrderByDescending(s => s.Email);
                    break;
                case "CreatedAt":
                    students = students.OrderBy(s => s.CreatedAt);
                    break;
                case "date_desc":
                    students = students.OrderByDescending(s => s.CreatedAt);
                    break;
                default:
                    students = students.OrderBy(s => s.FullName);
                    break;
            }

            //int _limit = (limit ?? 10);
            //int _page = (page ?? 1);

            //ViewBag.CurrentPage = _page;
            //ViewBag.Limit = _limit;
            //ViewBag.TotalPage = Math.Ceiling((double)(students.Count() / _limit));


            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(students.ToPagedList(pageNumber, pageSize));
            //return View(students.Skip((_page - 1) * _limit).Take(_limit).ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null || student.IsDeleted())
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RollNumber,FullName,Email,CreatedAt,UpdatedAt,DeletedAt,Status")] Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    student.CreatedAt = DateTime.Now;
                    student.UpdatedAt = DateTime.Now;
                    student.DeletedAt = DateTime.Now;
                    db.Students.Add(student);

                    db.SaveChanges();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = string.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            // raise a new exception nesting  
                            // the current instance as InnerException  
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
                return RedirectToAction("Index");
            }

            return View(student);
        }

        // GET: Students/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null || student.IsDeleted())
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RollNumber,FullName,Email,Status")] Student student)
        {
            if (student.RollNumber == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var existStudent = db.Students.Find(student.RollNumber);
            if (existStudent == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (ModelState.IsValid)
            {
                existStudent.FullName = student.FullName;
                existStudent.Email = student.Email;
                existStudent.Status = student.Status;
                existStudent.UpdatedAt = DateTime.Now;
                db.Students.AddOrUpdate(existStudent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null || student.IsDeleted())
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var existStudent = db.Students.Find(id);
            if (existStudent == null || existStudent.IsDeleted())
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            if (ModelState.IsValid)
            {
                existStudent.DeletedAt = DateTime.Now;
                existStudent.Status = StudentStatus.DELETED;
                db.Students.AddOrUpdate(existStudent);
                db.SaveChanges();
            }

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
