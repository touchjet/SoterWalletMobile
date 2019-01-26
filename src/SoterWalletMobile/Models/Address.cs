using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoterWalletMobile.Models
{
    public class Address
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int CoinId { get; set; }
        public uint Purpose { get; set; }
        public uint CoinType { get; set; }
        public uint Account { get; set; }
        public uint Change { get; set; }
        public uint AddressIndex { get; set; }
        public string AddressString { get; set; }
        public ulong Balance { get; set; }

        public Coin Coin { get; set; }
        public List<Transaction> Transactions { get; set; }
    }
}
