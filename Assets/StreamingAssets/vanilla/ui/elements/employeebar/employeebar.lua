local transform;

local company;
local job;

function OnScriptLoaded()
end

function OnScriptSet(_transform)
    transform = _transform;
    GetComponents();

    CompanyManager.OnPlayerCompanyChanged.add(HandleCompanyChanged);
    JobManager.OnJobsLoaded.add(HandleJobsLoaded);
    EmploymentManager.OnEmployed.add(HandleOnEmployed);

    EmployeeDictionary.OnAddedToPool.add(HandleEmployeeAddedToPool);
    EmployeeDictionary.OnRemovedFromPool.add(HandleEmployeeRemovedFromPool);
end

function GetComponents()
    job = {
        header = transform.Find("Header/Text").GetComponent("TextMeshProUGUI"),
        content = transform.Find("Employees/Scroll View/Viewport/Content"),
        available_employees = {
            header = transform.Find("AvailableEmployees/Header/Text").GetComponent("TextMeshProUGUI"),
            no = transform.Find("AvailableEmployees/NoAvailable").gameObject,
            no_text = transform.Find("AvailableEmployees/NoAvailable/Text").GetComponent("TextMeshProUGUI"),
            yes = transform.Find("AvailableEmployees/Scroll View").gameObject,
            content = transform.Find("AvailableEmployees/Scroll View/Viewport/Content"),
            prefab = transform.Find("AvailableEmployees/Scroll View/Viewport/Content/Employee").gameObject,
            open_tag = "",
            employees = {}
        },
        job_prefab = transform.Find("Employees/Scroll View/Viewport/Content/Job").gameObject,
        pool_parent = transform.Find("Employees"),
        employee_prefab = transform.Find("Employees/Employee").gameObject,
        jobs = {},
        employee_pool = {}
    };
    job.job_prefab.SetActive(false);
    job.employee_prefab.SetActive(false);
end

function HandleCompanyChanged(sender, _company)
    company = _company;
    Clear();

    local ids = EmploymentDictionary.SafeGetEmployeeIdsOfCompany(company.tag);
    local employees = EmployeeDictionary.SafeGetEmployees(ids);
    for i=1, #employees do
        Hire(employees[i]);
    end
end

function HandleJobsLoaded(sender, jobs)
    for i=1, #jobs do
        local object = Instantiate(job.job_prefab, job.content);
        object.name = jobs[i].tag;
        object.SetActive(true);

        job.jobs[jobs[i].tag] = {
            transform = object.transform,
            folded = true,
            header = object.transform.Find("Header/Text").GetComponent("TextMeshProUGUI"),
            no_employees = object.transform.Find("NoEmployees").gameObject,
            no_employees_text = object.transform.Find("NoEmployees/Text").GetComponent("TextMeshProUGUI"),
            yes_employees = object.transform.Find("Employees").gameObject,
            content = object.transform.Find("Employees/Scroll View/Viewport/Content"),
            employees = { }
        };

        AddFunctionality(object.transform.Find("Header/FoldButtons/Fold"), "HandleMenuFold", job.jobs[jobs[i].tag]);
        AddFunctionality(object.transform.Find("Header/FoldButtons/Unfold"), "HandleMenuUnfold", job.jobs[jobs[i].tag]);
        AddFunctionality(object.transform.Find("Header/Hire"), "HandleHireButtonPressed", jobs[i]);

        job.jobs[jobs[i].tag].header.text = LanguageManager.Translate(jobs[i].title.."_TITLE_PLURAL");

        local text_start = LanguageManager.Translate("NO_EMPLOYEE_OF_TYPE_START");
        local text_end = LanguageManager.Translate("NO_EMPLOYEE_OF_TYPE_END");
        job.jobs[jobs[i].tag].no_employees_text.text = text_start.." "..LanguageManager.Translate(jobs[i].title.."_TITLE_PLURAL"):lower().." "..text_end;
    end
end

function HandleOnEmployed(sender, data)
    if not (data.company == company.tag) then return end

    local employee = EmployeeDictionary.SafeGet(data.employee);
    if (employee == nil) then return end

    local menu = job.jobs[employee.Job.tag];
    if data.hired then
        -- HIRE
        Hire(employee);
    else
        -- FIRE
        local employee_element = menu.employees[""..employee.id];
        employee_element.transform.gameObject.SetActive(false);
        employee_element.transform.SetParent(job.pool_parent);
        job.employee_pool[#job.employee_pool+1] = employee_element;
        menu.employees[""..employee.id] = nil;
    end

    if not menu.folded then
        ShowHideEmployees(menu, true);
        ForceUpdateLayoutGroup();
    end
end

function Hire(employee)
    local menu = job.jobs[employee.Job.tag];
    local object = CreateOrGetAvailableEmployeeObject(menu);
    object.transform.gameObject.SetActive(true);
    object.transform.localScale = Vector3(1,1,1);

    object.name.text = employee.name.." "..employee.surname;
    object.salary.text = ToCashString(employee.Salary);
    object.effects.text = employee.EffectsToString();
    ResetFunctionality(object.fire.gameObject.transform);
    AddFunctionality(object.fire.gameObject.transform, "HandleFireButtonPressed", employee);

    menu.employees[""..employee.id] = object;
end

function CreateOrGetAvailableEmployeeObject(for_job)
    if #job.employee_pool > 0 then
        local head = table.remove(job.employee_pool)
        SetParent(head.transform, for_job.content);
        return head;
    else
        local object = Instantiate(job.employee_prefab, for_job.content);
        return {
            transform = object.transform,
            image = object.transform.Find("Image").GetComponent("RawImage"),
            name = object.transform.Find("Name").GetComponent("TextMeshProUGUI"),
            salary = object.transform.Find("Salary").GetComponent("TextMeshProUGUI"),
            effects = object.transform.Find("Effects").GetComponent("TextMeshProUGUI"),
            fire = object.transform.Find("Fire/Text (TMP)").GetComponent("TextMeshProUGUI")
        };
    end
end

function HandleMenuFold(job)
    job.folded = true;
    ShowHideEmployees(job, false);
    -- Vertical layout group needs to be force updated
    -- This ensures everything has the correct layout
    ForceUpdateLayoutGroup();
end

function HandleMenuUnfold(job)
    job.folded = false;
    ShowHideEmployees(job, true);
    -- Vertical layout group needs to be force updated
    -- This ensures everything has the correct layout
    ForceUpdateLayoutGroup();
end

function ForceUpdateLayoutGroup()
    if transform.gameObject.activeInHierarchy then
        transform.gameObject.SetActive(false);
        transform.gameObject.SetActive(true);
    end
end

function ShowHideEmployees(job, show)
    if show then
        job.yes_employees.SetActive(not(job.content.childCount == 0));
        job.no_employees.SetActive(job.content.childCount == 0);
    else
        job.yes_employees.SetActive(false);
        job.no_employees.SetActive(false);
    end
end

function HandleHireButtonPressed(_job)
    job.available_employees.open_tag = _job.tag;
    job.available_employees.header.text = LanguageManager.Translate("AVAILABLE").." "..LanguageManager.Translate(_job.title.."_TITLE_PLURAL") 
    UpdatePoolWithTag(_job.tag);
end

function HandleFireButtonPressed(employee)
    EmploymentManager.Fire(company.tag, employee.id);
end

function Clear()
    for k, menu in pairs(job.jobs) do
        if not(menu == nil) then
            for _k, employee in pairs(menu.employees) do
                employee.transform.gameObject.SetActive(false);
                SetParent(employee.transform, job.pool_parent);
                job.employee_pool[#job.employee_pool+1] = employee;
            end
            menu.employees = {};
        end
    end
end

function HandleEmployeeAddedToPool(sender, employee)
    if job.available_employees.open_tag == employee.job_tag then
        UpdatePoolWithTag(employee.Job.tag);
    end
end

function HandleEmployeeRemovedFromPool(sender, employee)
    if job.available_employees.open_tag == employee.job_tag then
        UpdatePoolWithTag(employee.Job.tag);
    end
end

function UpdatePoolWithTag(tag)
    local employees = EmployeeDictionary.GetPoolWithTag(tag);

    if #employees > 0 then
        -- if we need any elemets, create
        if #job.available_employees.employees < #employees then
            CreateAvailableEmployeeElements(#employees - #job.available_employees.employees)
        end

        for i=1, #job.available_employees.employees do
            local element = job.available_employees.employees[i];

            if i > #employees then
                element.transform.gameObject.SetActive(false);
            else
                element.transform.gameObject.SetActive(true);
                element.name.text = employees[i].Name;
                element.salary.text = ToCashString(employees[i].Salary);
                element.effects.text = employees[i].EffectsToString();
            
                element.hire.text = LanguageManager.Translate("HIRE");
            
                ResetFunctionality(element.button);
                AddFunctionality(element.button, "HireEmployee", employees[i]);
            end
        end
    end
    job.available_employees.yes.SetActive(#employees > 0);
    job.available_employees.no.SetActive(not(#employees > 0));
end

function HireEmployee(employee)
    if company == nil then return end
    EmploymentManager.Hire(company.tag, employee.id);
    UpdatePoolWithTag(employee.job_tag);
end

function CreateAvailableEmployeeElements(amount)
    for i=1, amount do
        local object = Instantiate(job.available_employees.prefab, job.available_employees.content);
        object.gameObject.SetActive(true);

        job.available_employees.employees[#job.available_employees.employees + 1] = {
            transform = object.transform,
            name = object.transform.Find("Name").GetComponent("TextMeshProUGUI"),
            salary = object.transform.Find("Salary").GetComponent("TextMeshProUGUI"),
            effects = object.transform.Find("Effects").GetComponent("TextMeshProUGUI"),
            hire = object.transform.Find("Button/Text").GetComponent("TextMeshProUGUI"),
            button = object.transform.Find("Button"),
        }
    end
end
