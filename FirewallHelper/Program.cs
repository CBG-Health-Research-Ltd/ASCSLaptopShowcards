// create firewall rule

using WindowsFirewallHelper;

Console.WriteLine("Setting up firewall...");

try
{

    var ruleSearch =
        FirewallManager.Instance.Rules.SingleOrDefault(r =>
            r.Name == "IPSOS LaptopShowCard Firewall Rule Outbound");

    if (ruleSearch == null)
    {

        var ruleOutbound = FirewallManager.Instance.CreateApplicationRule(
            FirewallProfiles.Private | FirewallProfiles.Domain | FirewallProfiles.Public,
            @"IPSOS LaptopShowCard Firewall Rule Outbound",
            FirewallAction.Allow,
            @"C:\LaptopShowcards\LaptopShowcards.exe"
        );
        ruleOutbound.Direction = FirewallDirection.Outbound;

        FirewallManager.Instance.Rules.Add(ruleOutbound);
    }

    ruleSearch =
        FirewallManager.Instance.Rules.SingleOrDefault(
            r => r.Name == "IPSOS LaptopShowCard Firewall Rule Inbound");

    if (ruleSearch == null)
    {

        var ruleInbound = FirewallManager.Instance.CreateApplicationRule(
            FirewallProfiles.Private | FirewallProfiles.Domain | FirewallProfiles.Public,
            "IPSOS LaptopShowCard Firewall Rule Inbound",
            FirewallAction.Allow,
            @"C:\LaptopShowcards\LaptopShowcards.exe"
        );
        ruleInbound.Direction = FirewallDirection.Inbound;

        FirewallManager.Instance.Rules.Add(ruleInbound);

    }

    /*
    ruleSearch = FirewallManager.Instance.Rules.SingleOrDefault(r => r.Name == "IPSOS LaptopShowCard Port 8080");

    if (ruleSearch == null)
    {

        var rulePort = FirewallManager.Instance.CreatePortRule(
            FirewallProfiles.Private | FirewallProfiles.Domain | FirewallProfiles.Public,
            @"IPSOS LaptopShowCard Port 8080",
            FirewallAction.Allow,
            8080,
            FirewallProtocol.TCP
        );
        FirewallManager.Instance.Rules.Add(rulePort);

    }
    */
    Console.WriteLine("Done Setting up firewall...");
}
catch (Exception exp)
{
    Console.WriteLine("Firewall setup failed. Please setup and try again : ");
    Console.WriteLine("Error is :");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(exp.Message + "," + exp.StackTrace);

}

Console.WriteLine("Press any key to continue...");
Console.ReadKey();