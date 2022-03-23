local math = require("math")

local transform

local content = {
    background = nil,
    progress = nil,
    tip = nil,
}

local tips = {
    "GENERIC_TIP",
    "TIP_1"
}

local progress = {
    "GENERIC_PROGRESS"
}

local delta = 0;

function OnScriptLoaded()
    LoadManager.OnProgressStart.add(HandleProgressStart)
    LoadManager.OnLoadProgressed.add(HandleLoadProgressed)
    LoadManager.OnProgressFinish.add(HandleProgressFinish)

    math.randomseed(os.time())
    math.random(); math.random(); math.random();
end

function OnScriptSet(_transform)
    transform = _transform

    content = {
        background = transform.Find("Background").GetComponent("RawImage"),
        progress = transform.Find("Progress/Text").GetComponent("TextMeshProUGUI"),
        tip = transform.Find("Tips/Text").GetComponent("TextMeshProUGUI")
    }
end

function HandleLoadProgressed(sender, progress)
    delta = delta + progress.delta
    if (progress.message == "LANGUAGEMANAGER_LOGIC") then
        return;
    end

    if (delta >= 1) then
        delta = 0
        content.tip.text = LanguageManager.Translate(tips[math.random(1, #tips)])
    end

    content.progress.text = LanguageManager.Translate(progress.message)
end

function HandleProgressStart(sender)
    UIManager.loadMenu.gameObject.SetActive(true)
end

function HandleProgressFinish(sender)
    UIManager.loadMenu.gameObject.SetActive(false)
end
