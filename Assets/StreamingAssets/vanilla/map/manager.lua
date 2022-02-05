function OnScriptLoaded()
    MapManager.OnMapLoaded.add(HandleMapLoaded)
    MapManager.OnInvesmentsLoaded.add(HandleInvesmentsLoaded)
    MapManager.OnInvesmentClicked.add(HandleInvesmentClicked)
end

function OnRulesLoaded()
end

function OnContentLoaded()
end

-- mapTransform : Transform
function HandleMapLoaded(sender, map)
    -- Child 0 -> Chunks
    -- Child 1 -> Buildings
end

-- invesments : Dictionary<string, Transform[]>
function HandleInvesmentsLoaded(sender, invesments)
    -- invesments["property"][1].name -> Would return 0 because they are named with their id's and Lua arrays start with 1
end

-- only phsyical invesments can be clicked
-- invesment - Map.Invesment type has { Tag:string, Id:int }
function HandleInvesmentClicked(sender, invesment)
end
