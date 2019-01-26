using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoterWalletMobile.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int AddressId { get; set; }
        public long BlockHeight { get; set; }
        public DateTime Received { get; set; }
        public string Hash { get; set; }
        public long Amount { get; set; }
        public long Fee { get; set; }

        public Address Address { get; set; }
    }
}
