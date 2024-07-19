declare namespace VueDefine {
    interface Component {
        data: () => object;
        props: Array<string> | object;
        computed: { [key: string]: Function | { get: Function; set: Function } };
        methods: { [key: string]: Function };
        watch: { [key: string]: string | Function | object | Array };
        emits: Array<string> | object;
        expose: Array<string>;
        render: Function;
        beforeCreate: Function;
        created: Function;
        beforeMount: Function;
        mounted: Function;
        beforeUpdate: Function;
        updated: Function;
        activated: Function;
        deactivated: Function;
        beforeUnmount: Function;
        unmounted: Function;
        errorCaptured: (err: Error, instance: Component, info: string) => ?boolean;
        directives: object;
        components: object;
        mixins: Array<object>;
        extends: object;
        provide: object | Function;
        inject: Array<string> | { [key: string]: string | Symbol | object };
        setup: Function;
        $data: object;
        $props: object;
        $el: any;
        $options: any;
        $parent: object;
        $root: object;
        $slots: object;
        $refs: object;
        $attrs: object;
        $watch(
            source: string | Function,
            callback: Function | Object,
            options?: { deep?: boolean; immediate?: boolean; flush?: string }
        ): Function;
        $emit: Function;
        $forceUpdate: Function;
        $nextTick(callback: Function): Promise<void>;
    }

    interface App {
        component(name: string, definition: Function | Component): object;
        config: object;
        directive(name: string, definition: Function | object): void;
        mixin: object;
        mount(rootContainer: Element | string, isHydrate: boolean): void;
        provide(key: string, value: any): App;
    }

    declare interface Vue {
        createApp(rootComponent: Component, props?: object): App;
        h(type: string, props: object, children: String | Array | object): any;
        defineComponent(definition: Component | Function): Component;
        defineCustomElement(definition: Component | Function): HTMLElement;
        resolveDynamicComponent(component: String | object): Component;
        nextTick(callback: Function): Promise<void>;
        version: string;
    }
}

declare const Vue: VueDefine.Vue;