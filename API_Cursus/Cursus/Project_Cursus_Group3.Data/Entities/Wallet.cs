using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_Cursus_Group3.Data.Entities
{
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }

        [Required]
        public double? Balance { get; set; }
        [Required]
        public DateTime TransactionTime { get; set; }
        public string? UserName { get; set; }

        [ForeignKey("UserName")]
        public User User { get; set; }

        public virtual ICollection<Transactions> Transactions { get; set; }
    }
}
