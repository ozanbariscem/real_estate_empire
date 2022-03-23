local transform

local menu = {
    new_game = {
        transform = nil,
        company_prefab = nil,
        content = nil,
        caution = nil,
        file_name_input = nil,
    }
}

onClicks = {
    { "MainMenu/SideBar/BottomBar/Socials/Youtube", "OpenYoutube" },
    { "MainMenu/SideBar/BottomBar/Socials/Twitter", "OpenTwitter" },
    { "MainMenu/SideBar/BottomBar/Socials/Discord", "OpenDiscord" },
    
    { "MainMenu/SideBar/Buttons/Continue/Button", "Continue" },
    { "MainMenu/SideBar/Buttons/Exit/Button", "Quit" },

    { "NewGameMenu/Content/Botbar/Button", "NewGame" },
    { "LoadGameMenu/Content/Botbar/Button", "LoadGame" }
}

onHovers = {
    { "NewGameMenu/Content/Botbar/File/Header/Image", "OnSaveFileCautionHovered" }
}

onValueChanges = {
    { "NewGameMenu/Content/Botbar/File/FileNameInput", "HandleInputChanged" }
}

local last_clicked_company = nil
local last_clicked_file = nil

function OnScriptLoaded()
    OwnershipManager.OnOwnershipsLoaded.add(HandleOwnershipsLoaded)
    SaveFileManager.OnSaveFileDatasLoaded.add(HandleSaveFilesLoaded)

    CompanyManager.OnPlayerCompanyChanged.add(HandleCompanyChanged)
end

function OnScriptSet(_transform)
    transform = _transform
    
    Translate()
    GetReferences()
end

function Translate()
    transform.Find("MainMenu/SideBar/Buttons/Continue/Button/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("CONTINUE")
    transform.Find("MainMenu/SideBar/Buttons/NewGame/Button/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("NEW_GAME")
    transform.Find("MainMenu/SideBar/Buttons/LoadGame/Button/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("LOAD_GAME")
    transform.Find("MainMenu/SideBar/Buttons/Multiplayer/Button/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("MULTIPLAYER")
    transform.Find("MainMenu/SideBar/Buttons/Settings/Button/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("SETTINGS")
    transform.Find("MainMenu/SideBar/Buttons/Exit/Button/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("EXIT")

    transform.Find("MainMenu/SideBar/BottomBar/Login/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("LOGGED_IN_AS")

    transform.Find("NewGameMenu/Content/Topbar/Header/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("CHOOSE_COMPANY")
    transform.Find("NewGameMenu/Content/Botbar/Button/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("PLAY")
    transform.Find("NewGameMenu/Content/Botbar/File/Header/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("SAFE_NAME")
    transform.Find("NewGameMenu/Content/Botbar/File/FileNameInput/Text Area/Placeholder").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("FILE_NAME_PLACEHOLDER")

    transform.Find("NewGameMenu/Content/CompanyList/Scroll View/Viewport/Content/CompanyCardPrefab/Content/Information/Finance/Head/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("FINANCE")
    transform.Find("NewGameMenu/Content/CompanyList/Scroll View/Viewport/Content/CompanyCardPrefab/Content/Information/Investment/Head/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("INVESTMENTS")
    transform.Find("NewGameMenu/Content/CompanyList/Scroll View/Viewport/Content/CompanyCardPrefab/Content/Information/Finance/Funds/Key").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("FUNDS")
    transform.Find("NewGameMenu/Content/CompanyList/Scroll View/Viewport/Content/CompanyCardPrefab/Content/Information/Finance/Assets/Key").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("ASSETS")
    transform.Find("NewGameMenu/Content/CompanyList/Scroll View/Viewport/Content/CompanyCardPrefab/Content/Information/Finance/Debt/Key").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("DEBT")
    transform.Find("NewGameMenu/Content/CompanyList/Scroll View/Viewport/Content/CompanyCardPrefab/Content/Information/Finance/Networth/Key").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("NETWORTH")
    transform.Find("NewGameMenu/Content/CompanyList/Scroll View/Viewport/Content/CompanyCardPrefab/Content/Information/Investment/Property/Key").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("PROPERTY")
    
    transform.Find("LoadGameMenu/Content/Topbar/Header/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("CHOOSE_FILE")
    transform.Find("LoadGameMenu/Content/FileList/Scroll View/Viewport/Content/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("NO_SAVE_FILE_NOTICE")
    transform.Find("LoadGameMenu/Content/Company/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("CHOOSE_FILE_TO_SEE_COMPANY")
    transform.Find("LoadGameMenu/Content/Botbar/Button/Text").GetComponent("TextMeshProUGUI").text = LanguageManager.Translate("PLAY")
end

function GetReferences()
    menu = {
        new_game = {
            transform = transform.Find("NewGameMenu"),
            company_prefab = transform.Find("NewGameMenu/Content/CompanyList/Scroll View/Viewport/Content/CompanyCardPrefab/").gameObject,
            content = transform.Find("NewGameMenu/Content/CompanyList/Scroll View/Viewport/Content/"),
            caution = transform.Find("NewGameMenu/Content/Botbar/File/Header/Image"),
            file_name_input = transform.Find("NewGameMenu/Content/Botbar/File/FileNameInput").GetComponent("TMP_InputField"),
            companies = {}
        },
        load_game = {
            transform = transform.Find("LoadGameMenu"),
            file_prefab = transform.Find("LoadGameMenu/Content/FileList/Scroll View/Viewport/Content/SaveFilePrefab").gameObject,
            text = transform.Find("LoadGameMenu/Content/FileList/Scroll View/Viewport/Content/Text"),
            content = transform.Find("LoadGameMenu/Content/FileList/Scroll View/Viewport/Content"),
            company = {
                transform = transform.Find("LoadGameMenu/Content/Company"),
                text = transform.Find("LoadGameMenu/Content/Company/Text").GetComponent("TextMeshProUGUI"),
                card = {
                    transform = transform.Find("LoadGameMenu/Content/Company/CompanyCardPrefab"),
                    name = transform.Find("LoadGameMenu/Content/Company/CompanyCardPrefab/Content/Name").GetComponent("TextMeshProUGUI"),
                    funds = transform.Find("LoadGameMenu/Content/Company/CompanyCardPrefab/Content/Information/Finance/Funds/Value").GetComponent("TextMeshProUGUI"),
                    assets = transform.Find("LoadGameMenu/Content/Company/CompanyCardPrefab/Content/Information/Finance/Assets/Value").GetComponent("TextMeshProUGUI"),
                    debt = transform.Find("LoadGameMenu/Content/Company/CompanyCardPrefab/Content/Information/Finance/Debt/Value").GetComponent("TextMeshProUGUI"),
                    networth = transform.Find("LoadGameMenu/Content/Company/CompanyCardPrefab/Content/Information/Finance/Networth/Value").GetComponent("TextMeshProUGUI"),
                    property = transform.Find("LoadGameMenu/Content/Company/CompanyCardPrefab/Content/Information/Investment/Property/Value").GetComponent("TextMeshProUGUI"),
                }
            },
            files = {}
        }
    }

    menu.new_game.company_prefab.SetActive(false)
end

function HandleOwnershipsLoaded(sender, companies)
    local companies = CompanyDictionary.GetAsList(); 

    if #menu.new_game.companies < #companies then
        local prefab = menu.new_game.company_prefab
        for i=1, #companies - #menu.new_game.companies do
            local object = Instantiate(prefab, menu.new_game.content)

            AddFunctionality(object.transform.Find("Content"), "HandleCompanyClicked", object.transform)
            menu.new_game.companies[#menu.new_game.companies+1] = {
                transform = object.transform,
                name = object.transform.Find("Content/Name").GetComponent("TextMeshProUGUI"),
                finance = {
                    funds = object.transform.Find("Content/Information/Finance/Funds/Value").GetComponent("TextMeshProUGUI"),
                    assets = object.transform.Find("Content/Information/Finance/Assets/Value").GetComponent("TextMeshProUGUI"),
                    debt = object.transform.Find("Content/Information/Finance/Debt/Value").GetComponent("TextMeshProUGUI"),
                    networth = object.transform.Find("Content/Information/Finance/Networth/Value").GetComponent("TextMeshProUGUI"),
                },
                investments = {
                    property = object.transform.Find("Content/Information/Investment/Property/Value").GetComponent("TextMeshProUGUI"),
                }
            }
        end
    end

    for i=1, #companies do
        local el = menu.new_game.companies[i]
        if i <= #companies then
            el.transform.name = companies[i].tag
            
            el.name.text = companies[i].name
            el.finance.funds.text = ToCashString(companies[i].cash)
            el.finance.assets.text = ToCashString(companies[i].assetsWorth)
            el.finance.debt.text = ToCashString(companies[i].debt)
            el.finance.networth.text = ToCashString(companies[i].Networth)

            el.investments.property.text = OwnershipDictionary.GetTotalAssetsOfCompany(companies[i].tag, "property")

            el.transform.gameObject.SetActive(true)
        else
            el.transform.gameObject.SetActive(false)
        end
    end

    HandleCompanyClicked(menu.new_game.companies[1].transform)
end

function HandleCompanyClicked(transform)
    local company = CompanyDictionary.Dictionary[transform.name]

    if last_clicked_company != nil then
        last_clicked_company.Find("Content/Border").gameObject.SetActive(false)
    end
    transform.Find("Content/Border").gameObject.SetActive(true)
    last_clicked_company = transform

    menu.new_game.file_name_input.text = company.tag.."_"..os.date("%Y-%m-%d")
end

function HandleInputChanged(text)
    menu.new_game.file_name_input.text = text
    menu.new_game.caution.gameObject.SetActive(SaveFileManager.SaveFileAlreadyExists(text))
end

function OnSaveFileCautionHovered()
    return {
        header = LanguageManager.Translate("FILE_EXISTS"),
        description = LanguageManager.Translate("FILE_EXISTS_DESCRIPTION")
    }
end

function HandleSaveFilesLoaded(sender, files)
    if #files < 1 then
        menu.load_game.text.gameObject.SetActive(true)
        return
    end

    menu.load_game.text.gameObject.SetActive(false)
    if #menu.load_game.files < #files then
        local prefab = menu.load_game.file_prefab
        for i=1, (#files - #menu.load_game.files) do
            local object = Instantiate(prefab, menu.load_game.content)

            menu.load_game.files[#menu.load_game.files+1] = {
                transform = object.transform,
                name = object.transform.Find("Name").GetComponent("TextMeshProUGUI"),
                company = object.transform.Find("Company").GetComponent("TextMeshProUGUI"),
                date = object.transform.Find("Date").GetComponent("TextMeshProUGUI"),
                data = nil
            }
            AddFunctionality(object.transform, "HandleFileClicked", menu.load_game.files[#menu.load_game.files])
        end
    end

    for i=1, #files do
        local el = menu.load_game.files[i]
        if i <= #files then
            el.transform.name = files[i].name
            
            el.name.text = files[i].name
            el.company.text = files[i].name
            el.date.text = files[i].date
            el.data = files[i]
            el.transform.gameObject.SetActive(true)
        else
            el.transform.gameObject.SetActive(false)
        end
    end
end

function HandleFileClicked(file)
    if last_clicked_file != nil then
        last_clicked_file.transform.Find("Border").gameObject.SetActive(false)
    end
    file.transform.Find("Border").gameObject.SetActive(true)
    last_clicked_file = file

    SaveFileManager.LoadSaveFile(file.data)

    menu.load_game.company.text.gameObject.SetActive(false)
    menu.load_game.company.card.transform.gameObject.SetActive(true)
end

function HandleCompanyChanged(sender, company)
    menu.load_game.company.card.name.text = company.name

    menu.load_game.company.card.funds.text = ToCashString(company.cash)
    menu.load_game.company.card.assets.text = ToCashString(company.assetsWorth)
    menu.load_game.company.card.debt.text = ToCashString(company.debt)
    menu.load_game.company.card.networth.text = ToCashString(company.Networth)

    menu.load_game.company.card.property.text = OwnershipDictionary.GetTotalAssetsOfCompany(company.tag, "property") 
end

function NewGame()
    CompanyManager.ChangeCompany(last_clicked_company.name)
    SaveFileManager.SetCurrentSaveFileName(menu.new_game.file_name_input.text)
    Play()
end

function LoadGame()
    -- incase player changed their mind and got back to load scene
    SaveFileManager.LoadSaveFile(last_clicked_file.data)
    Play()
end

function Continue()
    SaveFileManager.LoadLastSaveFile()
    Play()
end

function Play()
    transform.gameObject.SetActive(false)
    UIManager.gameMenu.gameObject.SetActive(true)
end
