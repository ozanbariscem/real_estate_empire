function OnScriptLoaded()
    MapManager.OnMapLoaded.add(HandleMapLoaded)
    MapManager.OnDistrictsLoaded.add(HandleDistrictsLoaded)

    MapManager.OnDistrictClicked.add(HandleDistrictClicked)
    MapManager.OnDistrictDoubleClicked.add(HandleDistrictDoubleClicked)
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

function HandleDistrictClicked(sender, district)
end

function HandleDistrictDoubleClicked(sender, district)
end
