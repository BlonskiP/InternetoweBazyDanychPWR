using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskRegiser.Core;
using TaskRegiser.Core.Entities;

namespace TaskRegister.View.Controllers
{
    [Authorize(Roles = RolesResource.Policy.AllUsers)]
    public class ProjectTasksController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectTasksController(AppDbContext context)
        {
            _context = context;
        }
        [Authorize(Roles = RolesResource.Policy.AdminOnly)]
        // GET: ProjectTasks
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.ProjectTasks.Include(p => p.Employee).Include(p => p.project);
            return View(await appDbContext.ToListAsync());
        }
        [Authorize(Roles = RolesResource.Policy.AllUsers)]
        public async Task<IActionResult> GetTasks()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var appDbContext = await _context.ProjectTasks.Include(p => p.Employee).Where(p => p.Employee.Id == user).Include(p => p.project).ToListAsync();
            return View(appDbContext);
        }
        // GET: ProjectTasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectTask = await _context.ProjectTasks
                .Include(p => p.Employee)
                .Include(p => p.project)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (projectTask == null)
            {
                return NotFound();
            }

            return View(projectTask);
        }

        // GET: ProjectTasks/Create
        [Authorize(Roles = RolesResource.Policy.AdminOnly)]
        public IActionResult Create()
        {
            ViewData["EmployeeFK"] = new SelectList(_context.Users, "Id", "UserName");
            ViewData["ProjectFK"] = new SelectList(_context.Projects, "ID", "Name");
            return View();
        }
        [Authorize(Roles = RolesResource.Policy.AllUsers)]
        public IActionResult CreateEmployeeTask()
        {
            ViewData["ProjectFK"] = new SelectList(_context.Projects, "ID", "Name");
            return View();
        }

        // POST: ProjectTasks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesResource.Policy.AdminOnly)]
        public async Task<IActionResult> Create([Bind("ID,Name,Approved,DateEnd,EmployeeFK,ProjectFK,DateStart")] ProjectTask projectTask)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeFK"] = new SelectList(_context.Users, "Id", "Id", projectTask.EmployeeFK);
            ViewData["ProjectFK"] = new SelectList(_context.Projects, "ID", "Name", projectTask.ProjectFK);
            return View(projectTask);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesResource.Policy.AllUsers)]
        public async Task<IActionResult> CreateEmployeeTask([Bind("ID,Name,Approved,DateEnd,ProjectFK,DateStart")] ProjectTask projectTask)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);
            projectTask.EmployeeFK = user;
            projectTask.Employee = _context.Users.Where(us => us.Id == user).First();
            projectTask.project = _context.Projects.Where(pro => pro.ID == projectTask.ProjectFK).First();
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            ModelState.Clear();
            var modelValidation = TryValidateModel(projectTask, nameof(ProjectTask));
            if (modelValidation)
            {
                _context.Add(projectTask);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GetTasks));
            }
            ViewData["EmployeeFK"] = new SelectList(_context.Users, "Id", "UserName", projectTask.EmployeeFK);
            ViewData["ProjectFK"] = new SelectList(_context.Projects, "ID", "Name", projectTask.ProjectFK);
            return View(projectTask);
        }

        // GET: ProjectTasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectTask = await _context.ProjectTasks.FindAsync(id);
            if (projectTask == null)
            {
                return NotFound();
            }
            ViewData["EmployeeFK"] = new SelectList(_context.Users, "Id", "UserName", projectTask.EmployeeFK);
            ViewData["ProjectFK"] = new SelectList(_context.Projects, "ID", "Name", projectTask.ProjectFK);
            return View(projectTask);
        }

        // POST: ProjectTasks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Approved,DateEnd,EmployeeFK,ProjectFK,DateStart")] ProjectTask projectTask)
        {
            if (id != projectTask.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectTask);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectTaskExists(projectTask.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(GetTasks));
            }
            ViewData["EmployeeFK"] = new SelectList(_context.Users, "Id", "UserName", projectTask.EmployeeFK);
            ViewData["ProjectFK"] = new SelectList(_context.Projects, "ID", "Name", projectTask.ProjectFK);
            return View(projectTask);
        }

        // GET: ProjectTasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var projectTask = await _context.ProjectTasks
                .Include(p => p.Employee)
                .Include(p => p.project)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (projectTask == null)
            {
                return NotFound();
            }

            return View(projectTask);
        }

        // POST: ProjectTasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var projectTask = await _context.ProjectTasks.FindAsync(id);
            _context.ProjectTasks.Remove(projectTask);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectTaskExists(int id)
        {
            return _context.ProjectTasks.Any(e => e.ID == id);
        }
    }
}
