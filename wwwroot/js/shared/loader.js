/**
 * Skeleton Loader Component
 */
const Loader = {
  renderTableSkeleton(columnCount = 5, rowCount = 4) {
    let rows = '';
    for (let r = 0; r < rowCount; r++) {
      let cols = '';
      for (let c = 0; c < columnCount; c++) {
        cols += `<td><div class="skeleton" style="height: 18px; width: 80%;"></div></td>`;
      }
      rows += `<tr>${cols}</tr>`;
    }
    return rows;
  },

  renderCardSkeleton(count = 3) {
    let cards = '';
    for (let i = 0; i < count; i++) {
      cards += `
        <div class="card">
          <div class="skeleton" style="height: 24px; width: 60%; margin-bottom: 12px;"></div>
          <div class="skeleton" style="height: 16px; width: 90%; margin-bottom: 8px;"></div>
          <div class="skeleton" style="height: 16px; width: 40%;"></div>
        </div>
      `;
    }
    return cards;
  }
};
