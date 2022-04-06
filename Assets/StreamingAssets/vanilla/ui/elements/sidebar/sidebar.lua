local transform;
local openbar;

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform;
end

function OnActivate(param)
    if not(openbar == nil) then
        openbar.SetParent(transform.parent);
        openbar.gameObject.SetActive(false);
    end
    openbar = UIManager.OpenMenu("GameMenu/"..param, transform);
    openbar.SetParent(transform);

    transform.SetSiblingIndex(1);
end

function OnDeactivate(param)
end
