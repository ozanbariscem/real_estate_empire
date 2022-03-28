function OnScriptLoaded()
    MapManager.OnMapLoaded.add(HandleMapLoaded)
    MapManager.OnDistrictsLoaded.add(HandleDistrictsLoaded)
end

function OnRulesLoaded()
end

function OnContentLoaded()
end

-- mapTransform : Transform
function HandleMapLoaded(sender, map)
end

-- districts : Dictionary<string, Map.District>
function HandleDistrictsLoaded(sender, districts)
end
