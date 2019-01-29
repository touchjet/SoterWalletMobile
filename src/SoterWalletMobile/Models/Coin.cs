using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SoterDevice.Contracts;

namespace SoterWalletMobile.Models
{
    public class Coin
    {
        public Coin()
        {

        }

        public Coin(CoinType coinType)
        {
            CoinName = coinType.CoinName;
            CoinShortcut = coinType.CoinShortcut;
            AddressType = coinType.AddressType;
            MaxfeeKb = coinType.MaxfeeKb;
            AddressTypeP2sh = coinType.AddressTypeP2sh;
            AddressTypeP2wpkh = coinType.AddressTypeP2wpkh;
            AddressTypeP2wsh = coinType.AddressTypeP2wsh;
            SignedMessageHeader = coinType.SignedMessageHeader;
            Bip44AccountPath = coinType.Bip44AccountPath;
            Forkid = coinType.Forkid;
            Decimals = coinType.Decimals;
            ContractAddress = coinType.ContractAddress;
            GasLimit = coinType.GasLimit;
            XpubMagic = coinType.XpubMagic;
            XprvMagic = coinType.XprvMagic;
            Segwit = coinType.Segwit;
            ForceBip143 = coinType.ForceBip143;
            CurveName = coinType.CurveName;
            CashaddrPrefix = coinType.CashaddrPrefix;
            Bech32Prefix = coinType.Bech32Prefix;
            Decred = coinType.Decred;
            VersionGroupId = coinType.VersionGroupId;
            XpubMagicSegwitP2sh = coinType.XpubMagicSegwitP2sh;
            XpubMagicSegwitNative = coinType.XpubMagicSegwitNative;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string CoinName { get; set; }
        public string CoinShortcut { get; set; }
        public uint AddressType { get; set; }
        public ulong MaxfeeKb { get; set; }
        public uint AddressTypeP2sh { get; set; }
        public uint AddressTypeP2wpkh { get; set; }
        public uint AddressTypeP2wsh { get; set; }
        public string SignedMessageHeader { get; set; }
        public uint Bip44AccountPath { get; set; }
        public uint Forkid { get; set; }
        public uint Decimals { get; set; }
        public byte[] ContractAddress { get; set; }
        public byte[] GasLimit { get; set; }
        public uint XpubMagic { get; set; }
        public uint XprvMagic { get; set; }
        public bool Segwit { get; set; }
        public bool ForceBip143 { get; set; }
        public string CurveName { get; set; }
        public string CashaddrPrefix { get; set; }
        public string Bech32Prefix { get; set; }
        public bool Decred { get; set; }
        public uint VersionGroupId { get; set; }
        public uint XpubMagicSegwitP2sh { get; set; }
        public uint XpubMagicSegwitNative { get; set; }

        public bool Enabled { get; set; }
        public DateTime LastNetworkUpdate { get; set; }
        public ulong BlockHeight { get; set; }

        public List<Address> Addresses { get; set; }

        public string BalanceString
        {
            get
            {
                Decimal totalBalance = 0;
                if (Addresses.Count > 0)
                {
                    totalBalance = Addresses.Sum(a => (Decimal)a.ConfirmedBalance + (Decimal)a.UnconfirmedBalance) / (Decimal)Math.Pow(10, Decimals);
                }
                return String.Format("{0} {1}", totalBalance, CoinShortcut);
            }
        }
    }
}

