local transform;

local income;
local expense;
local total;
local loans;

local company;

onHovers = {
    { "Loans/Items/Scroll View/Viewport/Content/Headers/Items/Name", "HandleNameHeaderHovered" },
    { "Loans/Items/Scroll View/Viewport/Content/Headers/Items/Amount", "HandleAmountHeaderHovered" },
    { "Loans/Items/Scroll View/Viewport/Content/Headers/Items/PaymentsLeft", "HandlePaymentsLeftHeaderHovered" },
};

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform;

    GetComponents(transform);
    OnTranslate();

    CompanyManager.OnPlayerCompanyChanged.add(HandlePlayerCompanyChanged);
    Company.OnSalaryPaymentsCalculated.add(Update);
    Company.OnLoanPaymentsCalculated.add(Update);
    Company.OnAssetsWorthCalculated.add(Update);
    Company.OnDebtCalculated.add(Update);
    Company.OnCashChanged.add(Update);
end

function GetComponents(parent)
    income = {
        rent = {
            text = parent.Find("Income/Items/Rent/Text").GetComponent("TextMeshProUGUI"),
            value = parent.Find("Income/Items/Rent/Amount").GetComponent("TextMeshProUGUI")
        }
    };
    expense = {
        loan = {
            text = parent.Find("Expense/Items/Loans/Text").GetComponent("TextMeshProUGUI"),
            value = parent.Find("Expense/Items/Loans/Amount").GetComponent("TextMeshProUGUI")
        },
        employee = {
            text = parent.Find("Expense/Items/Employees/Text").GetComponent("TextMeshProUGUI"),
            value = parent.Find("Expense/Items/Employees/Amount").GetComponent("TextMeshProUGUI")
        }
    };
    total = {
        text = parent.Find("Total/Total").GetComponent("TextMeshProUGUI"),
        value = parent.Find("Total/Amount").GetComponent("TextMeshProUGUI")
    };
    loans = {
        text = parent.Find("Loans/Header/Text").GetComponent("TextMeshProUGUI"),
        headers = {
            name = parent.Find("Loans/Items/Scroll View/Viewport/Content/Headers/Items/Name").GetComponent("TextMeshProUGUI"),
            amount = parent.Find("Loans/Items/Scroll View/Viewport/Content/Headers/Items/Amount").GetComponent("TextMeshProUGUI"),
            payments_left = parent.Find("Loans/Items/Scroll View/Viewport/Content/Headers/Items/PaymentsLeft").GetComponent("TextMeshProUGUI"),
        },
        prefab = parent.Find("Loans/Items/Scroll View/Viewport/Content/Loan"),
        content = parent.Find("Loans/Items/Scroll View/Viewport/Content"),
        loans = {}
    };
end

function OnTranslate()
    income.rent.text.text = LanguageManager.Translate("RENT");
    expense.loan.text.text = LanguageManager.Translate("LOANS");
    expense.employee.text.text = LanguageManager.Translate("EMPLOYEES");
    total.text.text = LanguageManager.Translate("TOTAL");

    loans.text.text = LanguageManager.Translate("LOANS");
    loans.headers.name.text = LanguageManager.Translate("NAME");
    loans.headers.amount.text = LanguageManager.Translate("AMOUNT");
    loans.headers.payments_left.text = LanguageManager.Translate("PAYMENTS_LEFT");

    for i=1, #loans do
        loans.loans[i].pay.text = string.upper(LanguageManager.Translate("PAY"));
    end
end

function HandleNameHeaderHovered()
    return {
        header = LanguageManager.Translate("LOAN_NAME_TITLE"),
        description = LanguageManager.Translate("LOAN_NAME_DESCRIPTION"),
    };
end

function HandleAmountHeaderHovered()
    return {
        header = LanguageManager.Translate("LOAN_AMOUNT_TITLE"),
        description = LanguageManager.Translate("LOAN_AMOUNT_DESCRIPTION"),
    };
end

function HandlePaymentsLeftHeaderHovered()
    return {
        header = LanguageManager.Translate("LOAN_PAYMENTS_LEFT_TITLE"),
        description = LanguageManager.Translate("LOAN_PAYMENTS_LEFT_DESCRIPTION"),
    };
end

function HandlePlayerCompanyChanged(sender, _company)
    company = _company;
    Update(nil, company);
end

function Update(sender, _company)
    if company == nil then
        return;
    end

    if company.tag == _company.tag then
        UpdateFinance(company);
        UpdateLoans(company);
    end
end

function UpdateFinance(company)
    UpdateIncomes(company);
    UpdateExpenses(company);

    local text = "<color=#38A15D>";
    if company.MonthlyNet < 0 then
        text = "<color=#B75454>";
    end
    total.value.text = text..ToCashString(company.MonthlyNet).."</color>";
end

function UpdateIncomes(company)
    income.rent.value.text = "0";
end

function UpdateExpenses(company)
    expense.loan.value.text = ToCashString(company.loanPayments);
    expense.employee.value.text = ToCashString(company.salaryPayments);
end

function UpdateLoans(company)
    local loan_list = LoanDictionary.SafeGet(company.tag);

    if #loans.loans < #loan_list then
        CreateLoanElements(#loan_list - #loans.loans);
    end

    for i=1, #loans.loans do
        local el = loans.loans[i];
        if i <= #loan_list then
            el.name.text = loan_list[i].Type.name;
            el.amount.text = ToShortCashString(loan_list[i].amountLeft);
            local each = ToShortCashString(loan_list[i].Type.amount / loan_list[i].payment_left);
            el.payment_left.text = loan_list[i].payment_left.." ("..each.." "..LanguageManager.Translate("EACH")..")";
            el.transform.gameObject.SetActive(true);
        else
            el.transform.gameObject.SetActive(false);
        end
    end
end

function CreateLoanElements(amount)
    for i=1, amount do
        local object = Instantiate(loans.prefab, loans.content);
        object.gameObject.SetActive(true);
        
        loans.loans[#loans.loans + 1] = {
            transform = object, 
            name = object.transform.Find("Items/Name").GetComponent("TextMeshProUGUI"),
            amount = object.transform.Find("Items/Amount").GetComponent("TextMeshProUGUI"),
            payment_left = object.transform.Find("Items/PaymentsLeft").GetComponent("TextMeshProUGUI"),
            pay = object.transform.Find("Items/Pay/Text").GetComponent("TextMeshProUGUI")
        };
    end
end
