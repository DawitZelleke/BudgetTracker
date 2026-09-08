using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BudgetTracker.DataAccess;

namespace BudgetTracker.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly BudgetTrackerContext _context;

        public TransactionsController(BudgetTrackerContext context)
        {
            _context = context;
        }

        // GET: Transactions
        // sortOrder: "date" / "date_desc" / "description" / "description_desc" / "amount" / "amount_desc"
        public async Task<IActionResult> Index(string sortOrder)
        {
            // Toggle values used by the column header links in the Index view.
            // Default sort is Date descending (newest first), so Date toggles to ascending first.
            ViewData["DateSort"] = string.IsNullOrEmpty(sortOrder) || sortOrder == "date_desc"
                ? "date" : "date_desc";
            ViewData["DescriptionSort"] = sortOrder == "description"
                ? "description_desc" : "description";
            ViewData["AmountSort"] = sortOrder == "amount"
                ? "amount_desc" : "amount";
            ViewData["CurrentSort"] = sortOrder;

            var transactions = _context.Transactions
                .Include(t => t.Category)
                .AsQueryable();

            transactions = sortOrder switch
            {
                "date"             => transactions.OrderBy(t => t.TransactionDate),
                "description"      => transactions.OrderBy(t => t.Description),
                "description_desc" => transactions.OrderByDescending(t => t.Description),
                "amount"           => transactions.OrderBy(t => t.Amount),
                "amount_desc"      => transactions.OrderByDescending(t => t.Amount),
                _                  => transactions.OrderByDescending(t => t.TransactionDate), // default
            };

            return View(await transactions.AsNoTracking().ToListAsync());
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Amount,TransactionDate,TransactionType,CategoryId,Description")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.CreatedAt = DateTime.Now;
                transaction.UpdatedAt = DateTime.Now;
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Amount,TransactionDate,TransactionType,CategoryId,Description")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Load the tracked entity so CreatedAt is preserved and
                // UpdatedAt is refreshed server-side.
                var existing = await _context.Transactions.FindAsync(id);
                if (existing == null)
                {
                    return NotFound();
                }

                existing.Amount = transaction.Amount;
                existing.TransactionDate = transaction.TransactionDate;
                existing.TransactionType = transaction.TransactionType;
                existing.CategoryId = transaction.CategoryId;
                existing.Description = transaction.Description;
                existing.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", transaction.CategoryId);
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        // No GET Delete action and no Delete view: confirmation happens client-side
        // via a JavaScript confirm() dialog on the Index page.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
