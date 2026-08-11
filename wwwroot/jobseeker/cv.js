let currentCvDoc = null;

document.addEventListener('DOMContentLoaded', async () => {
  Guards.requireRole(CONSTANTS.ROLES.JOB_SEEKER);
  Navigation.renderShell(CONSTANTS.ROLES.JOB_SEEKER, 'cv');

  const dropZone = document.getElementById('dropZone');
  const fileInput = document.getElementById('cvFileInput');
  const fileInfoBox = document.getElementById('fileSelectedInfo');
  const fileNameSpan = document.getElementById('selectedFileName');
  const fileSizeSpan = document.getElementById('selectedFileSize');
  const clearFileBtn = document.getElementById('clearFileBtn');
  const uploadForm = document.getElementById('cvUploadForm');
  const submitBtn = document.getElementById('uploadSubmitBtn');

  // Load existing CV metadata
  await loadCurrentCv();

  // Drop zone events
  dropZone.addEventListener('click', () => fileInput.click());

  ['dragenter', 'dragover'].forEach(eventName => {
    dropZone.addEventListener(eventName, (e) => {
      e.preventDefault();
      dropZone.classList.add('dragover');
    });
  });

  ['dragleave', 'drop'].forEach(eventName => {
    dropZone.addEventListener(eventName, (e) => {
      e.preventDefault();
      dropZone.classList.remove('dragover');
    });
  });

  dropZone.addEventListener('drop', (e) => {
    const files = e.dataTransfer.files;
    if (files && files.length > 0) {
      fileInput.files = files;
      handleFileSelection(files[0]);
    }
  });

  fileInput.addEventListener('change', () => {
    if (fileInput.files && fileInput.files.length > 0) {
      handleFileSelection(fileInput.files[0]);
    }
  });

  clearFileBtn.addEventListener('click', () => {
    fileInput.value = '';
    fileInfoBox.classList.add('hidden');
    submitBtn.disabled = true;
  });

  function handleFileSelection(file) {
    const ext = file.name.split('.').pop().toLowerCase();
    if (ext !== 'pdf' && ext !== 'docx') {
      Toast.error('Invalid file format. Only PDF and DOCX files are allowed.');
      fileInput.value = '';
      return;
    }

    if (file.size > 5 * 1024 * 1024) {
      Toast.error('File size exceeds maximum limit of 5MB.');
      fileInput.value = '';
      return;
    }

    fileNameSpan.textContent = file.name;
    fileSizeSpan.textContent = `(${UI.formatFileSize(file.size)})`;
    fileInfoBox.classList.remove('hidden');
    submitBtn.disabled = false;
  }

  // Submit Upload
  uploadForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    if (!fileInput.files || fileInput.files.length === 0) return;

    submitBtn.disabled = true;
    submitBtn.querySelector('span').textContent = 'Uploading...';

    try {
      await Api.upload(CONSTANTS.API_ENDPOINTS.CV.BASE, fileInput.files[0]);
      Toast.success('CV uploaded successfully!');
      fileInput.value = '';
      fileInfoBox.classList.add('hidden');
      await loadCurrentCv();
    } catch (err) {
      Toast.error(err.message || 'Failed to upload CV');
    } finally {
      submitBtn.disabled = true;
      submitBtn.querySelector('span').textContent = 'Upload Resume Document';
    }
  });
});

async function loadCurrentCv() {
  const content = document.getElementById('cvDetailsContent');
  const badge = document.getElementById('cvStatusBadge');
  const uploadTitle = document.getElementById('uploadCardTitle');

  try {
    const cv = await Api.get(CONSTANTS.API_ENDPOINTS.CV.BASE);
    currentCvDoc = cv;

    if (!cv) {
      badge.className = 'badge badge-rejected';
      badge.textContent = 'No File Uploaded';
      uploadTitle.textContent = 'Upload CV Document';
      content.innerHTML = `
        <div class="empty-state" style="padding: var(--space-4);">
          <div class="empty-title" style="font-size: var(--font-size-base);">No CV Document Uploaded</div>
          <div class="empty-text text-sm mb-0">Upload a PDF or DOCX file below to allow employers to review your full resume.</div>
        </div>
      `;
      return;
    }

    badge.className = 'badge badge-selected';
    badge.textContent = 'Uploaded & Verified';
    uploadTitle.textContent = 'Replace Existing CV Document';

    content.innerHTML = `
      <div class="flex items-center justify-between p-3 bg-surface-secondary border rounded">
        <div>
          <div class="font-semibold text-main text-base mb-1">${UI.icons.cv} ${UI.escapeHtml(cv.originalFileName)}</div>
          <div class="text-xs text-muted">
            Uploaded on ${UI.formatDate(cv.uploadedAt)} • ${UI.formatFileSize(cv.fileSizeBytes)}
          </div>
        </div>
        <div class="flex items-center gap-2">
          <button class="btn btn-secondary btn-sm" onclick="downloadCv()">${UI.icons.download} Download Stream</button>
          <button class="btn btn-danger btn-sm" onclick="confirmDeleteCv()">${UI.icons.trash} Delete</button>
        </div>
      </div>
    `;
  } catch (err) {
    content.innerHTML = `<div class="text-danger text-sm">Failed to load CV information.</div>`;
  }
}

async function downloadCv() {
  try {
    Toast.info('Downloading CV stream...');
    const fileName = currentCvDoc ? currentCvDoc.originalFileName : 'resume.pdf';
    await Api.download(CONSTANTS.API_ENDPOINTS.CV.DOWNLOAD, fileName);
  } catch (err) {
    Toast.error('Download failed.');
  }
}

function confirmDeleteCv() {
  Modal.confirm({
    title: 'Delete CV Document',
    message: 'Are you sure you want to delete your uploaded CV? Applications requiring a CV will not be possible until a new file is uploaded.',
    confirmText: 'Delete CV',
    onConfirm: async () => {
      try {
        await Api.delete(CONSTANTS.API_ENDPOINTS.CV.BASE);
        Toast.success('CV document deleted.');
        await loadCurrentCv();
      } catch (err) {
        Toast.error(err.message || 'Failed to delete CV');
      }
    }
  });
}
