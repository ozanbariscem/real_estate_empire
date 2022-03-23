local transform

local menu = {
    name = nil,
    cash = nil,
    income = nil
}

onClicks = {
    { "Company/Button", "HandleCompanyMenuRequested" },
    { "Cash/Button", "HandleCompanyMenuRequested" }
}

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform

    menu = {
        name = transform.Find("Company/Text").GetComponent("TextMeshProUGUI"),
        cash = transform.Find("Cash/CashText").GetComponent("TextMeshProUGUI"),
        income = transform.Find("Cash/IncomeText").GetComponent("TextMeshProUGUI")
    }

    CompanyManager.OnPlayerCompanyChanged.add(HandlePlayerCompanyChanged)
end

function HandleCompanyMenuRequested()
    UIManager.OpenMenu("GameMenu/company", CompanyManager.PlayerCompany)
end

function HandlePlayerCompanyChanged(sender, company)
    menu.name.text = company.name
    menu.cash.text = ToCashString(company.cash)
end
