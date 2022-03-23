function OnScriptLoaded()
    CompanyManager.OnCompaniesLoaded.add(HandleCompaniesLoaded)
end

function OnRulesLoaded()
end

function OnContentLoaded()
end

-- companies : List<Company>
function HandleCompaniesLoaded(sender, companies)
end
