<template>
  <div class="guid-info">
    <div class="guid-info__intro">
      <slot />
    </div>
    <div class="guid-info__bg">
      <div class="guid-info__btn">
        <slot name="button" />
      </div>
    </div>
  </div>
</template>
<style lang="scss" scoped>
.guid-info {
  margin: 64px auto;
  width: 530px;
  text-align: center;
  &__intro {
    font-size: 16px;
    color: $main-color;

    .dark & {
      color: rgba(#fff, 0.6);
    }
    font-weight: 600;
    padding: 0 24px;
    line-height: 32px;
  }
  &__bg {
    min-height: 215px;
    border-bottom: 1px solid #192845;
    background: url("@/assets/images/empty_state_human.svg") no-repeat right
      59px bottom -2px;
    background-size: 170px auto;
    .dark & {
      border-bottom: 1px solid gray;
    }
  }
  &__btn {
    padding-top: 58px;
  }
}
</style>
