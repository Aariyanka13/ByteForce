const selectedSkillIds = new Set();

document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.EMPLOYER);
  Navigation.renderShell(CONSTANTS.ROLES.EMPLOYER, 'vacancy-create');

  const form = document.getElementById('createVacancyForm');
  const submitBtn = document.getElementById('submitVacancyBtn');

  // Load skills catalog
  await loadSkillsCatalog();

  const addCustomSkillBtn = document.getElementById('addCustomSkillBtn');
  const customSkillInput = document.getElementById('customSkillInput');

  async function handleAddCustomSkill() {
    const name = customSkillInput.value.trim();
    if (!name) return;

    try {
      addCustomSkillBtn.disabled = true;
      const skill = await Api.post(CONSTANTS.API_ENDPOINTS.SKILLS.BASE, { name });
      
      const existingChip = document.querySelector(`.skill-select-chip[data-id="${skill.id}"]`);
      if (existingChip) {
        if (!selectedSkillIds.has(skill.id)) {
          toggleSkillChip(existingChip, skill.id);
        }
      } else {
        const box = document.getElementById('skillsCatalogBox');
        const chip = document.createElement('div');
        chip.className = 'skill-select-chip selected';
        chip.setAttribute('data-id', skill.id);
        chip.onclick = function() { toggleSkillChip(this, skill.id); };
        chip.textContent = `+ ${skill.name}`;
        box.appendChild(chip);
        selectedSkillIds.add(skill.id);
      }
      customSkillInput.value = '';
      Toast.success(`Added skill "${skill.name}"`);
    } catch (err) {
      Toast.error(err.message || 'Failed to add custom skill');
    } finally {
      addCustomSkillBtn.disabled = false;
    }
  }

  if (addCustomSkillBtn && customSkillInput) {
    addCustomSkillBtn.addEventListener('click', handleAddCustomSkill);
    customSkillInput.addEventListener('keydown', (e) => {
      if (e.key === 'Enter') {
        e.preventDefault();
        handleAddCustomSkill();
      }
    });
  }

  form.addEventListener('submit', async (e) => {
    e.preventDefault();
    let isValid = Validators.validateForm(form);

    const skillsFeedback = document.getElementById('skillsFeedback');
    if (selectedSkillIds.size === 0) {
      skillsFeedback.style.display = 'block';
      isValid = false;
    } else {
      skillsFeedback.style.display = 'none';
    }

    if (!isValid) return;

    submitBtn.disabled = true;

    try {
      await Api.post(CONSTANTS.API_ENDPOINTS.VACANCIES.BASE, {
        title: document.getElementById('title').value.trim(),
        description: document.getElementById('description').value.trim(),
        location: document.getElementById('location').value.trim(),
        requiredExperienceYears: parseFloat(document.getElementById('experience').value) || 0,
        requiredEducationLevel: document.getElementById('education').value,
        skillIds: Array.from(selectedSkillIds)
      });

      Toast.success('Job vacancy published successfully!');
      setTimeout(() => {
        window.location.href = CONSTANTS.ROUTES.EMPLOYER.VACANCIES;
      }, 500);
    } catch (err) {
      Toast.error(err.message || 'Failed to publish vacancy');
      submitBtn.disabled = false;
    }
  });
});

async function loadSkillsCatalog() {
  const box = document.getElementById('skillsCatalogBox');
  try {
    const skills = await Api.get(CONSTANTS.API_ENDPOINTS.SKILLS.BASE);
    if (!skills || skills.length === 0) {
      box.innerHTML = `<div class="text-muted text-sm">No skills found in master catalog.</div>`;
      return;
    }

    box.innerHTML = skills.map(s => `
      <div class="skill-select-chip" data-id="${s.id}" onclick="toggleSkillChip(this, ${s.id})">
        + ${UI.escapeHtml(s.name)}
      </div>
    `).join('');
  } catch (err) {
    box.innerHTML = `<div class="text-danger text-sm">Failed to load skills catalog.</div>`;
  }
}

function toggleSkillChip(element, skillId) {
  if (selectedSkillIds.has(skillId)) {
    selectedSkillIds.delete(skillId);
    element.classList.remove('selected');
  } else {
    selectedSkillIds.add(skillId);
    element.classList.add('selected');
  }
}
