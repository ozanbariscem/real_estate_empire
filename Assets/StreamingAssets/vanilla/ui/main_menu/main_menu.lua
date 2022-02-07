local transform

local mainMenu -- Transform
local singlePlayerMenu -- Transform

-- Sidebar on singleplayer menu
local newGameMenu -- Transform
local loadGameMenu -- Transform

local newGameContent -- Transform
local loadGameContent -- Transform

local newGamePrompt -- Transform
local fileNameInput -- InputField

local fileExistsPrompt -- Transform


onClicks = {
    { "MainMenu/Buttons/SinglePlayer", "HandleSinglePlayerButtonPressed"},
    { "MainMenu/Buttons/Exit", "HandleExitButtonPressed"},

    { "SinglePlayerMenu/BackButton", "HandleBackToMainMenuButtonPressed"},
    { "SinglePlayerMenu/PlayButton", "HandlePlayButtonPressed"},
    { "SinglePlayerMenu/SavesMenu/Topbar/NewGameButton", "HandleNewGameButtonPressed"},
    { "SinglePlayerMenu/SavesMenu/Topbar/LoadGameButton", "HandleLoadGameButtonPressed"},

    { "SinglePlayerMenu/NewGamePrompt/NewGame/BackButton", "HandleBackToSinglePlayerMenuButtonPressed"},
    { "SinglePlayerMenu/NewGamePrompt/NewGame/StartButton", "HandleNewGameStartButtonPressed"},

    { "SinglePlayerMenu/NewGamePrompt/FileExistsPrompt/Yes", "HandleFileExistsYesButtonPressed"},
    { "SinglePlayerMenu/NewGamePrompt/FileExistsPrompt/No", "HandleFileExistsNoButtonPressed"},

}

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform

    mainMenu = transform.Find("MainMenu")
    singlePlayerMenu = transform.Find("SinglePlayerMenu")
    
    newGameMenu = transform.Find("SinglePlayerMenu/SavesMenu/NewGameMenu")
    loadGameMenu = transform.Find("SinglePlayerMenu/SavesMenu/LoadGameMenu")

    newGameContent = newGameMenu.Find("Scroll View/Viewport/Content")
    loadGameContent = loadGameMenu.Find("Scroll View/Viewport/Content")

    newGamePrompt = singlePlayerMenu.Find("NewGamePrompt")
    fileExistsPrompt = newGamePrompt.Find("FileExistsPrompt")

    fileNameInput = newGamePrompt.Find("NewGame/FileNameInput").GetComponent("TMP_InputField")
end

function OnClickEventsSet()
end

function HandleSinglePlayerButtonPressed()
    mainMenu.gameObject.SetActive(false)
    singlePlayerMenu.gameObject.SetActive(true)
end

function HandleBackToMainMenuButtonPressed()
    mainMenu.gameObject.SetActive(true)
    singlePlayerMenu.gameObject.SetActive(false)
end

function HandleNewGameButtonPressed()
    newGameMenu.gameObject.SetActive(true)
    loadGameMenu.gameObject.SetActive(false)
end

function HandleLoadGameButtonPressed()
    newGameMenu.gameObject.SetActive(false)
    loadGameMenu.gameObject.SetActive(true)
end

function HandlePlayButtonPressed()
    ConsoleRunCommand("log_error YOU FORGOT TO HANDLE SAVE LOADING CASE ON MAIN_MENU.LUA YOU DUMMY")

    newGamePrompt.gameObject.SetActive(true)
end

function HandleBackToSinglePlayerMenuButtonPressed()
    newGamePrompt.gameObject.SetActive(false)
end

function HandleNewGameStartButtonPressed()
    ConsoleRunCommand("log_error YOU FORGOT TO HANDLE FILE EXISTS CASE ON MAIN_MENU.LUA YOU DUMMY")

    fileExistsPrompt.gameObject.SetActive(true)
end

function HandleFileExistsYesButtonPressed()
end

function HandleFileExistsNoButtonPressed()
    fileExistsPrompt.gameObject.SetActive(false)
end

function HandleExitButtonPressed()
    Quit()
end
