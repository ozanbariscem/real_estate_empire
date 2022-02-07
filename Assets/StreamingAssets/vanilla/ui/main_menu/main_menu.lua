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

local saveFilePrefab -- GameObject 

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

local a_file_is_selected = false;
local scenario_file_selected = false;

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform
    SetObjectReferences()
    SetEventHandlers()
end

function OnClickEventsSet()
end

function SetObjectReferences()
    mainMenu = transform.Find("MainMenu")
    singlePlayerMenu = transform.Find("SinglePlayerMenu")
    
    newGameMenu = transform.Find("SinglePlayerMenu/SavesMenu/NewGameMenu")
    loadGameMenu = transform.Find("SinglePlayerMenu/SavesMenu/LoadGameMenu")

    newGameContent = newGameMenu.Find("Scroll View/Viewport/Content")
    loadGameContent = loadGameMenu.Find("Scroll View/Viewport/Content")

    newGamePrompt = singlePlayerMenu.Find("NewGamePrompt")
    fileExistsPrompt = newGamePrompt.Find("FileExistsPrompt")

    fileNameInput = newGamePrompt.Find("NewGame/FileNameInput").GetComponent("TMP_InputField")

    saveFilePrefab = singlePlayerMenu.Find("SavesMenu/SaveFilePrefab").gameObject
end

function SetEventHandlers()
    SaveFileManager.OnScenarioDatasLoaded.add(HandleScenarioDatasLoaded)
    SaveFileManager.OnSaveFileDatasLoaded.add(HandleSaveFileDatasLoaded)
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

    if a_file_is_selected and not scenario_file_selected then
        UIManager.gameMenu.gameObject.SetActive(true)
        UIManager.mainMenu.gameObject.SetActive(false)
    else
        newGamePrompt.gameObject.SetActive(true)
    end
end

function HandleBackToSinglePlayerMenuButtonPressed()
    newGamePrompt.gameObject.SetActive(false)
end

function HandleNewGameStartButtonPressed()
    ConsoleRunCommand("log_error YOU FORGOT TO HANDLE FILE EXISTS CASE ON MAIN_MENU.LUA YOU DUMMY")

    if SaveFileManager.SaveFileAlreadyExists(fileNameInput.text) then
        fileExistsPrompt.gameObject.SetActive(true)
    else
        SaveFileManager.SetCurrentSaveFileName(fileNameInput.text)

        UIManager.gameMenu.gameObject.SetActive(true)
        UIManager.mainMenu.gameObject.SetActive(false)
    end
end

function HandleFileExistsYesButtonPressed()
    SaveFileManager.SetCurrentSaveFileName(fileNameInput.text)
    UIManager.gameMenu.gameObject.SetActive(true)
    UIManager.mainMenu.gameObject.SetActive(false)
end

function HandleFileExistsNoButtonPressed()
    fileExistsPrompt.gameObject.SetActive(false)
end

function HandleScenarioDatasLoaded(sender, scenarios)
    for i=1, #scenarios do
        local object = Instantiate(saveFilePrefab, newGameContent)
        SetSaveFile(object.transform, scenarios[i], true)
    end
end

function HandleSaveFileDatasLoaded(sender, saveFiles)
    for i=1, #saveFiles do
        local object = Instantiate(saveFilePrefab, loadGameContent)
        SetSaveFile(object.transform, saveFiles[i], false)
    end
end

function SetSaveFile(transform, data, is_scenario)
    transform.Find("FileNameText").GetComponent("TextMeshProUGUI").text = data.name
    transform.Find("DescriptionText").GetComponent("TextMeshProUGUI").text = data.path
    transform.Find("DateText").GetComponent("TextMeshProUGUI").text = data.date
    transform.gameObject.SetActive(true)

    AddFunctionality(transform.Find("Button"), "HandleSaveFileButtonClick", data)
    if is_scenario then
        AddFunctionality(transform.Find("Button"), "HandleScenarioFileSelected", nil)
    else
        AddFunctionality(transform.Find("Button"), "HandleSaveFileSelected", nil)
    end
end

function HandleSaveFileButtonClick(data)
    SaveFileManager.SetCurrentSaveFileName(data.name)
    SaveFileManager.LoadSaveFile(data)
end

function HandleSaveFileSelected()
    a_file_is_selected = true
    scenario_file_selected = false
end

function HandleScenarioFileSelected()
    a_file_is_selected = true
    scenario_file_selected = true
end

function HandleExitButtonPressed()
    Quit()
end
