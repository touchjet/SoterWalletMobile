using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoterWalletMobile.Models
{
    public class WalletDevice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string BleGuid { get; set; }
        public string Label { get; set; }
        public string Uuid { get; set; }
        public string Model { get; set; }
        public bool Initialized { get; set; }
        public uint MajorVersion { get; set; }
        public uint MinorVersion { get; set; }
        public uint PatchVersion { get; set; }
        public string Language { get; set; }
        public string BootloaderHash { get; set; }
        public string FirmwareHash { get; set; }
    }
}
