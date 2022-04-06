local transform;
local company;

local menu;

onClicks = {
    { "Items/Cash/Button", "HandleFinanceButtonPressed" },
    { "Items/Employee/Button", "HandleEmployeeButtonPressed" }
};

onHovers = {
    { "Items/Cash", "HandleFinanceHovered" },
    { "Items/Property", "HandlePropertiesHovered" },
    { "Items/Employee", "HandleEmployeeHovered" },
    { "Company", "HandleCompanyHovered" }
};

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform;

    menu = {
        cash = transform.Find("Items/Cash/Cash").GetComponent("TextMeshProUGUI"),
        income = transform.Find("Items/Cash/Income").GetComponent("TextMeshProUGUI"),
        properties = transform.Find("Items/Property/Text").GetComponent("TextMeshProUGUI"),
        employees = transform.Find("Items/Employee/Text").GetComponent("TextMeshProUGUI"),
        company = transform.Find("Company/Text").GetComponent("TextMeshProUGUI")
    };

    CompanyManager.OnPlayerCompanyChanged.add(HandlePlayerCompanyChange);

    Company.OnExpensesCalculated.add(HandleExpensesCalculated);

    Company.OnCashChanged.add(HandleCashChanged);
    
    Company.OnEmployeeLimitChanged.add(UpdateEmployeeLimit);
    Company.OnPropertyLimitChanged.add(UpdatePropertyLimit);
end

function UpdateCash()
    local text = "<color=#ECECEC>";
    if company.Cash < 0 then
        text = "<color=#B75454>";
    end
    menu.cash.text = text..ToCashString(company.Cash).."</color>";
end

function UpdateIncome()
    local text = "<color=#38A15D>";
    if company.MonthlyNet < 0 then
        text = "<color=#B75454>";
    end
    menu.income.text = text..ToCashString(company.MonthlyNet).."</color>";
end

function UpdateEmployeeLimit(sender, _company)
    if company == _company then
        menu.employees.text = company.EmployeeCount.."/"..company.EmployeeLimit;
    end
end

function UpdatePropertyLimit(sender, _company)
    if company == _company then
        menu.properties.text = company.PropertyCount.."/"..company.PropertyLimit;
    end
end

function HandleExpensesCalculated(sender, _company)
    if company == _company then
        UpdateIncome();
    end
end

function HandlePlayerCompanyChange(sender, _company)
    company = _company;

    UpdateCash();
    UpdateIncome();
    UpdateEmployeeLimit(nil, company);
    UpdatePropertyLimit(nil, company);
    menu.company.text = company.name;
end

function HandleCashChanged(sender, _company)
    if company == _company then
        UpdateCash();
    end
end

function HandleFinanceButtonPressed()
    UIManager.OpenMenu("GameMenu/sidebar", "financebar");
end

function HandleEmployeeButtonPressed()
    UIManager.OpenMenu("GameMenu/sidebar", "employeebar");
end

function HandleFinanceHovered()
    return {
        header = LanguageManager.Translate("FINANCE_HOVER_TITLE"),
        description = "\n"..LanguageManager.Translate("FINANCE_HOVER_DESCRIPTION")
    };
end

function HandlePropertiesHovered()
    return {
        header = LanguageManager.Translate("PROPERTY_HOVER_TITLE"),
        description = "\n"..LanguageManager.Translate("PROPERTY_HOVER_DESCRIPTION")
    };
end

function HandleEmployeeHovered()
    return {
        header = LanguageManager.Translate("EMPLOYEE_HOVER_TITLE"),
        description = "\n"..LanguageManager.Translate("EMPLOYEE_HOVER_DESCRIPTION")
    };
end

function HandleCompanyHovered()
    return {
        header = LanguageManager.Translate("COMPANY_HOVER_TITLE"),
        description = "\n"..LanguageManager.Translate("COMPANY_HOVER_DESCRIPTION")
    };
end
