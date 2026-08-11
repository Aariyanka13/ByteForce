let allSkills = [];
let selectedSkillsMap = new Map(); // id -> { id, name }

document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.JOB_SEEKER);
  Navigation.renderShell(CONSTANTS.ROLES.JOB_SEEKER, 'skills');

  const searchInput = document.getElementById('skillSearchInput');
  const saveBtn = document.getElementById('saveSkillsBtn');

  // 1. Fetch current candidate profile skills & master skills catalog
  try {
    const [profile, catalog] = await Promise.all([
      Api.get(CONSTANTS.API_ENDPOINTS.JOBSEEKER.PROFILE),
      Api.get(CONSTANTS.API_ENDPOINTS.SKILLS.BASE)
    ]);

    allSkills = catalog || [];
    if (profile.skills) {
      profile.skills.forEach(s => selectedSkillsMap.set(s.id, s));
    }

    renderSelectedSkills();
    renderAvailableSkills(allSkills);
  } catch (err) {
    Toast.error('Failed to load skills catalog');
  }

  // 2. Filter skills as user types
  searchInput.addEventListener('input', (e) => {
    const query = e.target.value.toLowerCase().trim();
    const filtered = allSkills.filter(s => s.name.toLowerCase().includes(query));
    renderAvailableSkills(filtered);
  });

  const addCustomSkillBtn = document.getElementById('addCustomSkillBtn');

  async function handleAddCustomSkill() {
    const name = searchInput.value.trim();
    if (!name) return;

    try {
      addCustomSkillBtn.disabled = true;
      const skill = await Api.post(CONSTANTS.API_ENDPOINTS.SKILLS.BASE, { name });
      
      if (!allSkills.some(s => s.id === skill.id)) {
        allSkills.push(skill);
      }
      selectedSkillsMap.set(skill.id, skill);
      renderSelectedSkills();
      renderAvailableSkills(allSkills);
      searchInput.value = '';
      Toast.success(`Added & selected skill "${skill.name}"`);
    } catch (err) {
      Toast.error(err.message || 'Failed to add custom skill');
    } finally {
      addCustomSkillBtn.disabled = false;
    }
  }

  if (addCustomSkillBtn) {
    addCustomSkillBtn.addEventListener('click', handleAddCustomSkill);
    searchInput.addEventListener('keydown', (e) => {
      if (e.key === 'Enter') {
        e.preventDefault();
        handleAddCustomSkill();
      }
    });
  }

  // 3. Save selection
  saveBtn.addEventListener('click', async () => {
    saveBtn.disabled = true;
    const skillIds = Array.from(selectedSkillsMap.keys());

    try {
      await Api.put(CONSTANTS.API_ENDPOINTS.JOBSEEKER.SKILLS, { skillIds });
      Toast.success('Skills saved successfully!');
    } catch (err) {
      Toast.error(err.message || 'Failed to save skills selection');
    } finally {
      saveBtn.disabled = false;
    }
  });
});

function renderSelectedSkills() {
  const box = document.getElementById('selectedSkillsBox');
  const countSpan = document.getElementById('selectedCount');
  const emptyText = document.getElementById('emptySelectedText');

  countSpan.textContent = selectedSkillsMap.size;

  if (selectedSkillsMap.size === 0) {
    emptyText.classList.remove('hidden');
    box.innerHTML = '';
    box.appendChild(emptyText);
    return;
  }

  box.innerHTML = Array.from(selectedSkillsMap.values()).map(skill => `
    <div class="skill-chip">
      <span>${UI.escapeHtml(skill.name)}</span>
      <button class="skill-chip-remove" onclick="removeSkill(${skill.id})" title="Remove skill">&times;</button>
    </div>
  `).join('');
}

function renderAvailableSkills(skillsList) {
  const box = document.getElementById('availableSkillsBox');
  if (!skillsList || skillsList.length === 0) {
    box.innerHTML = `<div class="text-muted text-sm py-2">No matching skills found.</div>`;
    return;
  }

  box.innerHTML = skillsList.map(skill => {
    const isSelected = selectedSkillsMap.has(skill.id);
    return `
      <button type="button" class="available-skill-tag ${isSelected ? 'selected' : ''}" onclick="addSkill(${skill.id}, '${UI.escapeHtml(skill.name)}')">
        + ${UI.escapeHtml(skill.name)}
      </button>
    `;
  }).join('');
}

function addSkill(id, name) {
  if (!selectedSkillsMap.has(id)) {
    selectedSkillsMap.set(id, { id, name });
    renderSelectedSkills();
    renderAvailableSkills(allSkills);
  }
}

function removeSkill(id) {
  if (selectedSkillsMap.has(id)) {
    selectedSkillsMap.delete(id);
    renderSelectedSkills();
    renderAvailableSkills(allSkills);
  }
}
