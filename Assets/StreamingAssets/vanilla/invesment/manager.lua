function OnScriptLoaded()
    InvesmentManager.OnTypesLoaded.add(HandleTypesLoaded)
    InvesmentManager.OnInvesmentsLoaded.add(HandleInvesmentsLoaded)
end

function OnRulesLoaded()
end

function OnContentLoaded()
end

-- types : Dictionary<string, Type>
function HandleTypesLoaded(sender, types)
end

-- invesments : Dictionary<string, List<Invesment>>
function HandleInvesmentsLoaded(sender, invesments)
end
