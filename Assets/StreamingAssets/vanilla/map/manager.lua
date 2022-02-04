function OnScriptLoaded()
end

-- mapTransform : Transform
function OnMapLoaded(map)
    -- Child 0 -> Chunks
    -- Child 1 -> Buildings
end

-- invesments : Dictionary<string, Transform[]>
function OnInvesmentsLoaded(invesments)
    -- invesments["property"][1].name -> Would return 0 because they are named with their id's and Lua arrays start with 1
end

-- only phsyical invesments can be clicked
-- invesment - Map.Invesment type has { Tag:string, Id:int }
function OnInvesmentClicked(invesment)
end
