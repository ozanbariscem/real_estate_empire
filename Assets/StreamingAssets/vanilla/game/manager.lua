-- Managers are private on GameManager class so it's important to catch these at OnManagersInitialized
local gameManager
local timeManager
local mapManager
local invesmentManager

function OnScriptLoaded()
end

function OnManagersInitialized(game, time, map, invesment)
    gameManager = game
    timeManager = time
    mapManager = map
    invesmentManager = invesment
end

-- Gets called every gameManager.ScriptUpdateInterval if it's not set to -1
-- delta is the time past since last time this Lua function was called
-- delta keeps adding up even if interval is set to -1
-- If you want this to simulate 60fps set interval to 0.01666666666=1second/60frames
function OnUpdate(delta)
    ConsoleRunCommand("log "..delta)
    -- Only thing that runs on Update is Time and Input
    -- Because we have so many events you need NOT to use OnUpdate for any game logic stuff
end

-- default value is -1 = Don't update 
function ChangeScriptUpdateInterval(interval)
    if interval != -1 and interval < 0 then
        return
    end
    gameManager.ScriptUpdateInterval = interval
end