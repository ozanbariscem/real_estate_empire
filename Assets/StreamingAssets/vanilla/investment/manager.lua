function OnScriptLoaded()
    InvestmentManager.OnTypesLoaded.add(HandleTypesLoaded)
    InvestmentManager.OnInvestmentsLoaded.add(HandleInvestmentsLoaded)
end

function OnRulesLoaded()
end

function OnContentLoaded()
end

-- types : Dictionary<string, Type>
function HandleTypesLoaded(sender, types)
end

-- invesments : Dictionary<string, List<Investment>>
function HandleInvestmentsLoaded(sender, investments)
end
