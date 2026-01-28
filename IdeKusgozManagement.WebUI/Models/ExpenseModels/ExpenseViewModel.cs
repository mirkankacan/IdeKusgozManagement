namespace IdeKusgozManagement.WebUI.Models.ExpenseModels
{
    public class ExpenseViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int ExpenseType { get; set; }
        public string TypeText { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}