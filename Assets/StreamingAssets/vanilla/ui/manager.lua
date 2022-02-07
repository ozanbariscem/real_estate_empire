function OnScriptLoaded()
    LoadManager.OnProgressFinish.add(HandleProgressFinish)
end

function OnRulesLoaded()
end

function OnContentLoaded()
end

function HandleProgressFinish()
    UIManager.mainMenu.gameObject.SetActive(true)
end
