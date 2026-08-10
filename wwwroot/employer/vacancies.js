const vacancyForm = document.getElementById("vacancyForm");
const submitButton =vacancyForm.querySelector('button[type="submit"]');
const vacancyMessage = document.getElementById("vacancyMessage");
const vacancyList = document.getElementById("vacancyList");
const skillsContainer = document.getElementById("skillsContainer");

const titleInput = document.getElementById("title");
const descriptionInput = document.getElementById("description");
const locationInput = document.getElementById("location");
const experienceInput = document.getElementById("requiredExperienceYears");
const educationInput = document.getElementById("requiredEducationLevel");

const token = localStorage.getItem("token");
let editingVacancyId = null;

async function loadSkills() {
    if (!token) {
        skillsContainer.innerHTML =
            "<p>Please log in as an employer.</p>";
        return;
    }

    try {
        const response = await fetch("/api/skills", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) {
            skillsContainer.innerHTML =
                "<p>Unable to load skills.</p>";
            return;
        }

        const skills = await response.json();

        if (skills.length === 0) {
            skillsContainer.innerHTML =
                "<p>No skills are available.</p>";
            return;
        }

        skillsContainer.innerHTML = skills
            .map(skill => `
                <label class="skill-option">
                    <input
                        type="checkbox"
                        name="skillIds"
                        value="${skill.id}">
                    <span>${skill.name}</span>
                </label>
            `)
            .join("");
    }
    catch (error) {
        skillsContainer.innerHTML =
            "<p>An error occurred while loading skills.</p>";
    }
}

async function loadVacancies() {
    if (!token) {
        vacancyList.innerHTML =
            "<p>Please log in as an employer.</p>";
        return;
    }

    try {
        const response = await fetch("/api/vacancies", {
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (!response.ok) {
            vacancyList.innerHTML =
                "<p>Unable to load vacancies.</p>";
            return;
        }

        const vacancies = await response.json();

        if (vacancies.length === 0) {
            vacancyList.innerHTML =
                "<p>No vacancies posted yet.</p>";
            return;
        }

        vacancyList.innerHTML = vacancies
            .map(vacancy => `
                <article class="vacancy-card">
                    <div class="vacancy-card-header">
                        <div>
                            <h3>${escapeHtml(vacancy.title)}</h3>
                            <p>${escapeHtml(vacancy.location ?? "Location not specified")}</p>
                        </div>

                        <span class="status-badge ${vacancy.isClosed ? "closed" : "open"}">
                            ${vacancy.isClosed ? "Closed" : "Open"}
                        </span>
                    </div>

                    <p class="vacancy-description">
                        ${escapeHtml(vacancy.description)}
                    </p>

                    <div class="vacancy-details">
                        <span>
                            Experience:
                            ${vacancy.requiredExperienceYears} year(s)
                        </span>

                        <span>
                            Education:
                            ${getEducationName(
                vacancy.requiredEducationLevel)}
                        </span>
                    </div>

                   ${
                vacancy.isClosed
                    ? ""
                    : `
            <div class="vacancy-actions">
                <button
                    type="button"
                    class="edit-vacancy-button"
                    data-id="${vacancy.id}">
                    Edit Vacancy
                </button>

                <button
                    type="button"
                    class="close-vacancy-button"
                    data-id="${vacancy.id}">
                    Close Vacancy
                </button>
            </div>
        `
}
                </article>
            `)
            .join("");

        document
            .querySelectorAll(".close-vacancy-button")
            .forEach(button => {
                button.addEventListener("click", async () => {
                    await closeVacancy(
                        Number(button.dataset.id));
                });
            });
        document
            .querySelectorAll(".edit-vacancy-button")
            .forEach(button => {
                button.addEventListener("click", async () => {
                    await loadVacancyForEdit(
                        Number(button.dataset.id));
                });
            });
    }
    catch (error) {
        vacancyList.innerHTML =
            "<p>An error occurred while loading vacancies.</p>";
    }
}
async function loadVacancyForEdit(vacancyId) {
    try {
        const response = await fetch(
            `/api/vacancies/${vacancyId}`,
            {
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

        if (!response.ok) {
            vacancyMessage.textContent =
                "Unable to load vacancy for editing.";
            return;
        }

        const vacancy = await response.json();

        editingVacancyId = vacancy.id;
        submitButton.textContent = "Update Vacancy";

        titleInput.value = vacancy.title ?? "";
        descriptionInput.value = vacancy.description ?? "";
        locationInput.value = vacancy.location ?? "";

        experienceInput.value =
            vacancy.requiredExperienceYears;

        educationInput.value =
            vacancy.requiredEducationLevel;

        document
            .querySelectorAll(
                'input[name="skillIds"]')
            .forEach(input => {
                input.checked =
                    vacancy.skillIds.includes(
                        Number(input.value));
            });

        vacancyMessage.textContent =
            "Vacancy loaded for editing.";

        vacancyForm.scrollIntoView({
            behavior: "smooth",
            block: "start"
        });
    }
    catch (error) {
        vacancyMessage.textContent =
            "An error occurred while loading the vacancy.";
    }
}

vacancyForm.addEventListener(
    "submit",
    async function (event) {
        event.preventDefault();

        if (!token) {
            vacancyMessage.textContent =
                "Please log in as an employer.";
            return;
        }

        const selectedSkills = Array
            .from(
                document.querySelectorAll(
                    'input[name="skillIds"]:checked'))
            .map(input => Number(input.value));

        if (selectedSkills.length === 0) {
            vacancyMessage.textContent =
                "Please select at least one required skill.";
            return;
        }

        const requestBody = {
            title: titleInput.value.trim(),
            description: descriptionInput.value.trim(),
            location:
                locationInput.value.trim() || null,
            requiredExperienceYears:
                Number(experienceInput.value),
            requiredEducationLevel:
                Number(educationInput.value),
            skillIds: selectedSkills
        };

        const isEditing = editingVacancyId !== null;

        const url = isEditing
            ? `/api/vacancies/${editingVacancyId}`
            : "/api/vacancies";

        const method = isEditing
            ? "PUT"
            : "POST";

        try {
            const response = await fetch(url, {
                method: method,
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(requestBody)
            });

            if (!response.ok) {
                vacancyMessage.textContent =
                    isEditing
                        ? "Unable to update vacancy."
                        : "Unable to create vacancy.";
                return;
            }

            vacancyForm.reset();

            experienceInput.value = "0";
            educationInput.value = "0";

            vacancyMessage.textContent =
                isEditing
                    ? "Vacancy updated successfully."
                    : "Vacancy created successfully.";

            editingVacancyId = null;
            submitButton.textContent = "Create Vacancy";

            await loadVacancies();
        }
        catch (error) {
            vacancyMessage.textContent =
                "An error occurred while saving the vacancy.";
        }
    });
async function closeVacancy(vacancyId) {
    try {
        const response = await fetch(
            `/api/vacancies/${vacancyId}/close`,
            {
                method: "PATCH",
                headers: {
                    "Authorization": `Bearer ${token}`
                }
            });

        if (!response.ok) {
            vacancyMessage.textContent =
                "Unable to close vacancy.";
            return;
        }

        vacancyMessage.textContent =
            "Vacancy closed successfully.";

        await loadVacancies();
    }
    catch (error) {
        vacancyMessage.textContent =
            "An error occurred while closing the vacancy.";
    }
}

function getEducationName(value) {
    switch (value) {
        case 1:
            return "Certificate";
        case 2:
            return "Diploma";
        case 3:
            return "Bachelor";
        case 4:
            return "Master";
        default:
            return "No Requirement";
    }
}

function escapeHtml(value) {
    const div = document.createElement("div");
    div.textContent = value ?? "";
    return div.innerHTML;
}

loadSkills();
loadVacancies();