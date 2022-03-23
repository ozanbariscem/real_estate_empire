function OnScriptLoaded()
end

function OnScriptSet(_transform)
    -- Incase there is a prior element
    _transform.SetSiblingIndex(0)
end