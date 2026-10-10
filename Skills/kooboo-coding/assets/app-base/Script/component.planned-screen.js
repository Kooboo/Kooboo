/* kooboo-app-base:v2; role: managed-route-placeholder */
export default {
  name: 'PlannedScreen',
  koobooSource: { moduleUrl: import.meta.url },
  template: `
    <section class="app-preparing-screen" :data-screen-id="$route.meta.screenId" aria-busy="true">
      <div class="app-preparing-header" aria-hidden="true">
        <div class="app-preparing-heading">
          <div class="app-preparing-title"></div>
          <div class="app-preparing-description"></div>
        </div>
        <div class="app-preparing-actions">
          <div class="app-preparing-action"></div>
          <div class="app-preparing-action app-preparing-action--primary"></div>
        </div>
      </div>

      <div class="app-preparing-stats" aria-hidden="true">
        <div v-for="card in 4" :key="card" class="app-preparing-stat">
          <div class="app-preparing-stat__label"></div>
          <div class="app-preparing-stat__value"></div>
          <div class="app-preparing-stat__trend"></div>
        </div>
      </div>

      <div class="app-preparing-panel" aria-hidden="true">
        <div class="app-preparing-toolbar">
          <div class="app-preparing-search"></div>
          <div class="app-preparing-filter"></div>
          <div class="app-preparing-filter app-preparing-filter--short"></div>
          <div class="app-preparing-toolbar-action"></div>
        </div>
        <div class="app-preparing-table">
          <div class="app-preparing-table__header">
            <div v-for="column in 5" :key="column"></div>
          </div>
          <div v-for="row in 6" :key="row" class="app-preparing-table__row">
            <div class="app-preparing-cell app-preparing-cell--primary"></div>
            <div class="app-preparing-cell"></div>
            <div class="app-preparing-cell"></div>
            <div class="app-preparing-cell app-preparing-cell--status"></div>
            <div class="app-preparing-cell app-preparing-cell--action"></div>
          </div>
        </div>
      </div>
    </section>
  `
}
