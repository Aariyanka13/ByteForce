const form = document.getElementById("employerProfileForm");
const message = document.getElementById("message");

const companyNameInput = document.getElementById("companyName");
const industryInput = document.getElementById("industry");
const locationInput = document.getElementById("location");
const websiteInput = document.getElementById("website");
const descriptionInput = document.getElementById("description");

const token = localStorage.getItem("token");

let profileExists = false;

async function loadProfile() {
    if (!token) {
        message.textContent = "Please log in as an employer.";
        return;
    }

    try {
        const response = await fetch("/api/employer/profile", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.status === 404) {
            profileExists = false;
            return;
        }

        if (!response.ok) {
            message.textContent = "Unable to load employer profile.";
            return;
        }

        const profile = await response.json();

        profileExists = true;

        companyNameInput.value = profile.companyName ?? "";
        industryInput.value = profile.industry ?? "";
        locationInput.value = profile.location ?? "";
        websiteInput.value = profile.website ?? "";
        descriptionInput.value = profile.description ?? "";
    }
    catch (error) {
        message.textContent = "An error occurred while loading the profile.";
    }
}

form.addEventListener("submit", async function (event) {
    event.preventDefault();

    if (!token) {
        message.textContent = "Please log in as an employer.";
        return;
    }

    const requestBody = {
        companyName: companyNameInput.value.trim(),
        industry: industryInput.value.trim() || null,
        location: locationInput.value.trim() || null,
        website: websiteInput.value.trim() || null,
        description: descriptionInput.value.trim() || null
    };

    try {
        const response = await fetch("/api/employer/profile", {
            method: profileExists ? "PUT" : "POST",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(requestBody)
        });

        if (!response.ok) {
            message.textContent = "Unable to save employer profile.";
            return;
        }

        await response.json();

        profileExists = true;
        message.textContent = "Employer profile saved successfully.";
    }
    catch (error) {
        message.textContent = "An error occurred while saving the profile.";
    }
});

loadProfile();