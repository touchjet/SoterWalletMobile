#!/bin/bash
dotnet ef migrations add "$1" --project SoterWalletMobile --startup-project SoterWalletMobile.DbConfig
dotnet ef database update --project SoterWalletMobile --startup-project SoterWalletMobile.DbConfig