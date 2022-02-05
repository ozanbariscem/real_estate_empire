function OnScriptLoaded()
    InvestorManager.OnInvestorsLoaded.add(HandleInvestorsLoaded)
end

function OnRulesLoaded()
end

function OnContentLoaded()
end

-- investors : List<Investor>
function HandleInvestorsLoaded(sender, investors)
end
