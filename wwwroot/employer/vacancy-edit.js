let vacancyId = null;
const selectedSkillIds = new Set();

document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.EMPLOYER);
  Navigation.renderShell(CONSTANTS.ROLES.EMPLOYER, 'vacancies');

  const params = new URLSearchParams(window.location.search);
  vacancyId = params.get('id');

  if (!vacancyId) {
    window.location.href = CONSTANTS.ROUTES.EMPLOYER.VACANCIES;
    return;
  }

  const form = document.getElementById('editVacancyForm');
  const saveBtn = document.getElementById('saveVacancyBtn');

  // Load vacancy details & skills catalog
  await loadVacancyAndSkills();

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

    saveBtn.disabled = true;

    try {
      await Api.put(`${CONSTANTS.API_ENDPOINTS.VACANCIES.BASE}/${vacancyId}`, {
        title: document.getElementById('title').value.trim(),
        description: document.getElementById('description').value.trim(),
        location: document.getElementById('location').value.trim(),
        requiredExperienceYears: parseFloat(document.getElementById('experience').value) || 0,
        requiredEducationLevel: document.getElementById('education').value,
        skillIds: Array.from(selectedSkillIds)
      });

      Toast.success('Job vacancy updated successfully!');
      setTimeout(() => {
        window.location.href = CONSTANTS.ROUTES.EMPLOYER.VACANCIES;
      }, 500);
    } catch (err) {
      Toast.error(err.message || 'Failed to update vacancy');
      saveBtn.disabled = false;
    }
  });
});

async function loadVacancyAndSkills() {
  const box = document.getElementById('skillsCatalogBox');
  try {
    const [vacancy, skills] = await Promise.all([
      Api.get(`${CONSTANTS.API_ENDPOINTS.VACANCIES.BASE}/${vacancyId}`),
      Api.get(CONSTANTS.API_ENDPOINTS.SKILLS.BASE)
    ]);

    // Fill form
    document.getElementById('title').value = vacancy.title || '';
    document.getElementById('location').value = vacancy.location || '';
    document.getElementById('experience').value = vacancy.requiredExperienceYears !== null ? vacancy.requiredExperienceYears : '';
    if (vacancy.requiredEducationLevel) {
      document.getElementById('education').value = vacancy.requiredEducationLevel;
    }
    document.getElementById('description').value = vacancy.description || '';

    if (vacancy.requiredSkills) {
      vacancy.requiredSkills.forEach(s => selectedSkillIds.add(s.id));
    }

    // Render skills
    box.innerHTML = (skills || []).map(s => {
      const isSelected = selectedSkillIds.has(s.id);
      return `
        <div class="skill-select-chip ${isSelected ? 'selected' : ''}" data-id="${s.id}" onclick="toggleSkillChip(this, ${s.id})">
          + ${UI.escapeHtml(s.name)}
        </div>
      `;
    }).join('');
  } catch (err) {
    Toast.error('Failed to load vacancy information.');
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
