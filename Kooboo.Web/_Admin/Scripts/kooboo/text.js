(function() {
  var text = {
    common: {
      date: "Date",
      data: "Data",
      OK: "OK",
      close: "Close",
      cancel: "Cancel",
      save: "Save",
      start: "Start",
      config: "Config",
      create: "Create",
      yes: "YES",
      no: "NO",
      new: "new",
      name: "Name",
      add: "Add",
      select: "Select",
      amount: "Amount",
      setting: "Setting",
      preview: "Preview",
      edit: "Edit",
      remove: "Remove",
      clear: "Clear",
      lastModified: "Last modified",
      usedBy: "Used by",
      useFor: "Use for",
      user: "User",
      viewAllVersions: "View all versions",
      size: "Size",
      pageView: "Page view",
      online: "Online",
      externalLink: "External Link",
      remark: "Remark",
      sorry: "Sorry",
      notFound: "Not found",
      hello: "Hello",
      selectFile: "Select file",
      change: "Change",
      all: "All",
      empty: "Empty",
      next: "Next",
      previous: "Previous",
      choose: "Choose",
      upload: "Upload",
      uploadImages: "Upload images",
      from: "From",
      loading: "Loading",
      displayName: "Display name",
      key: "Key",
      value: "Value",
      placeholder: "Placeholder",
      type: "Type",
      username: "Username",
      buy: "Buy",
      pay: "Pay",
      failed: "Failed",
      price: "Price",
      author: "Author",
      description: "Description",
      tags: "Tags",
      import: "Import",
      back: "Back",
      use: "Use",
      confirm: "Confirm",
      detail: "Detail",
      balance: "Balance",
      enable: "Enable",
      enabled: "Enabled",

      API: "API",
      Code: "Code",
      HTML: "HTML",
      Types: "Types",
      Database: "Database",
      Content: "Content",
      Contents: "Contents",
      Labels: "Labels",
      Label: "Label",
      KeyValueStore: "Key-Value Store",
      HTMLblocks: "HTML blocks",
      HTMLblock: "HTML block",
      mediaLibrary: "Media library",
      diagnosis: "Diagnosis",
      sync: "Sync",
      siteLogs: "Site logs",
      visitorLogs: "Visitor logs",
      logVersions: "Versions",
      Disk: "Disk",
      Jobs: "Jobs",
      Domains: "Domains",
      Layouts: "Layouts",
      Layout: "Layout",
      Views: "Views",
      View: "View",
      Forms: "Forms",
      Form: "Form",
      Scripts: "Scripts",
      Script: "Script",
      Styles: "Styles",
      Style: "Style",
      KScript: "KScript",
      Group: "Group",
      Menus: "Menus",
      Menu: "Menu",
      ContentTypes: "Content types",
      ContentType: "Content type",
      Files: "Files",
      DataSources: "Data Sources",
      Page: "Page",
      Pages: "Pages",
      Image: "Image",
      Images: "Images",
      File: "File",
      Email: "Email",
      Relation: "Relation",
      Multilingual: "Multilingual",
      URL: "URL",
      delete: "Delete",
      deleteUsage: "Delete usage",
      external: "External",
      link: "Link",
      listName: "list",
      textContents: "Text contents",
      textContent: "Text content",
      contentFolders: "Content folders",
      contentFolder: "Content folder",
      Events: "Events",
      Event: "Event",
      Embedded: "Embedded",
      Settings: "Settings",
      Search: "Search",
      Debug: "Debug",
      SiteUser: "Site user",
      KConfig: "Kooboo Config",
      TableRelation: "Table Relation",
      TransferTask: "Transfer Task",
      Roles: "Roles",

      Demands: "Demands",
      Demand: "Demand",
      Services: "services",
      Service: "service",
      Discussion: "Discussion",
      Discussions: "Discussions",
      Apps: "Apps",
      Purchased: "Purchased",
      Hardwares: "Hardwares",
      Suppliers: "Suppliers",
      Supplier: "Supplier",
      Templates: "Templates",

      ProductManagement: "Products management",
      ProductTypes: "Product types",
      ProductType: "Product type",
      ProductCategories: "Product categories",

      unitPrice: "Unit price",
      totalPrice: "Total price",
      totalAmount: "Total amount",
      year: "Year",
      years: "Years",

      publish: "Publish",
      title: "Title",
      budget: "Budget",
      attachments: "Attachments",
      send: "Send",
      reply: "Reply",
      visitors: "visitors",
      startTime: "Start time",
      endMonth: "End month",
      renew: "Renew",
      chooseObject: "Choose object",
      chooseAction: "Choose action"
    },

    system: {
      serverName: "Server name",
      serverAddress: "Server address"
    },

    account: {
      sendResetEmailTip:
        "After submiting your request, we will send you a verification mail, please check in time",
      youAreNotAdmin:
        "You are not administrator of current organization, you do not have access to organization profile."
    },

    online: {
      yes: "online_YES",
      no: "online_NO"
    },

    confirm: {
      deleteItem: "Are you sure you want to delete this item?",
      deleteItems: "Are you sure you want to delete these items?",
      recallProposal: "Are you sure you want to delete your proposal?",
      deleteItemsWithRef:
        "This item is being used, deleting an in-use item will remove the reference from the container object, are you sure you want to continue?",
      overrideFile:
        "A file with the same name already exists, would you like to overwrite it?",
      exit: "Are you sure you want to exit?",
      beforeReturn:
        "You have unsaved changes, do you want to leave this page and discard your changes?",
      removeBinding: "Do you want to remove binging as well?",
      siteLogs: {
        blame:
          "You have selected multiple items, are you sure you want to continue?",
        restore: "Are you sure you want to restore the site to this point?"
      },
      disk: {
        clearLogs:
          "Are you sure you want to clean both system and visitor logs?",
        clearRepository: "Are you sure you want to clean system logs?",
        clearVisitorLog: "Are you sure you want to clean visitor logs"
      },
      search: {
        clean: "Are you sure you want to clean all the search index?",
        rebuild: "Are you sure you want to rebuild site's index?"
      },
      layoutEditor: {
        labelInside:
          "This element contains label binding, do you want to continue?"
      },
      restartExecution:
        "Debugger is finished, would you want to restart debugging?",
      changeDataCenter:
        "You are about to change your data center, are you sure you want to continue?",
      siteSynchronizing:
        "You are pulling updates from remote server, are you sure you want to abort?",
      invalidMeta: "Invalid meta. Please check your meta infomation.",
      demand: {
        takeProposal: "Are you sure you want to take this proposal?",
        acceptDelivery: "Are you sure you want to accept the job?"
      },
      deleteCategory:
        "This category is related to some products. Would you want to delete?",
      market: {
        changeCurrency:
          "Changing the default currency will transfer all your balance to new currency. Would you want to continue?",
        sure: "Are you sure?",
        completedOrder: "Are you sure your order has been completed?"
      }
    },

    alert: {
      uploadZipFile: "File type error, please upload zip file.",
      pleaseChooseAFolder: "Please choose a folder first",
      selectBeforePushing: "You should select items before pushing",
      exceptionOccuredAndDebuggerClosed: "Exception occured, debugger closed.",
      fileUpload: {
        emptyFile: "is(are) empty file.",
        invalidSuffix: "has(have) invalid suffix",
        invalidType: "is(are) invalid"
      },
      verificationEmailSent:
        "A verification Email has sent to the email you input. Please check in time."
    },

    validation: {
      required: "Field required",
      taken: "The value has been taken",
      tagNotAllow: "Tag not allowed.",
      notEqual: "Values are not the same.",
      usernameInvalid: "Invalid username",
      emailInvalid: "Invalid email",
      urlInvalid: "Invalid URL",
      containInvalidEmail: "The addresses have invalid email",
      nameInvalid:
        "Name should only contains letters, digits and underscore, and should start with a letter",
      folderNameInvalid: "Only letter, digits, -, _ and . is allowed",
      siteNameInvalid: "The name only allow letter, digits, -, _ and .",
      maxLength: "The maximum length is ",
      minLength: "The minimal length is ",
      earlierThan: "The start time should be earlier than end time",
      GreaterThan: "Budget should be greater than 0",
      greaterThan: "Input a value greater than ",
      lessThan: "Input a value less than ",
      inputError: "Input error",
      dataType: {
        Integer: "Integer",
        Decimal: "Decimal",
        Number: "Number",
        Guid: "Guid",
        String: "String",
        DateTime: "DateTime",
        Bool: "Bool"
      },
      objectNameRegex:
        "Name should start with a letter, can contain with . - _ and end with letters or digit.",
      contentTypeNameRegex:
        "Name should start with a letter, can contain with - _ . and end with letters or digit.",
      siteNameRegex:
        "Name should start with a letter or digit, can contain with - _ and end with letters or digit.",
      invaildPort: "Invalid port value. Only number is allowed.",
      portRange: "Port range: 0~65535",
      noValidIP: "Invalid IP"
    },

    error: {
      dataType: "The data type you input is incorrect",
      rangeError: "Range error: the min value should less than the max value."
    },

    correct: {
      dataType: {
        number: "The right data type is number"
      }
    },

    info: {
      upload: {
        success: "Upload succeeded.",
        fail: "Upload failed."
      },
      update: {
        success: "Update succeeded.",
        fail: "Update failed."
      },
      delete: {
        success: "Delete succeeded.",
        fail: "Delete failed."
      },
      save: {
        success: "Saved.",
        fail: "Save failed."
      },
      checkout: {
        success: "Checkout succeeded.",
        fail: "Checkout failed."
      },
      clean: {
        success: "Clean succeeded.",
        fail: "Clean failed."
      },
      payment: {
        success: "Payment succeeded.",
        fail: "Payment failed.",
        cancel: "Payment canceled.",
        tryAgain: "Make sure you have paid successfully and try again"
      },
      copy: {
        success: "Copy succeeded.",
        fail: "Copy failed."
      },
      pull: {
        success: "Pull succeeded.",
        fail: "Pull failed."
      },
      push: {
        success: "Push succeeded.",
        fail: "Push failed."
      },
      moveTo: {
        Trash: {
          success: "Move to Trash succeeded.",
          fail: "Move to Trash failed."
        },
        Spam: {
          success: "Move to Spam succeeded.",
          fail: "Move to Spam failed."
        },
        Sent: {
          success: "Move to Sent succeeded.",
          fail: "Move to Sent failed."
        },
        Inbox: {
          success: "Move to Inbox succeeded.",
          fail: "Move to Inbox failed."
        }
      },
      rebuild: {
        success: "Rebuild succeeded.",
        fail: "Rebuild failed."
      },
      enable: {
        success: "Enable succeeded.",
        fail: "Enable failed."
      },
      disable: {
        success: "Disable succeeded.",
        fail: "Disable failed."
      },
      recharge: {
        success: "Recharge succeeded",
        fail: "Recharge failed"
      },
      versionLogParameterMissing:
        "Parameters missing.<br/> Redirecting to site logs...",
      parameterMissing: "Parameter missing",
      menuNameRequired: "Name field is required.",
      fileSizeLessThan: "File size must be less than ",
      subDomainInfo: "Give your site a domain that other people can access.",
      startPulling: "Start pulling...",
      networkError: "Network error",
      checkServer: "please check your server.",
      inProgress: "This site is still downloading, please wait.",
      seleteExportStoreName:
        "Please select the content you want before exporting.",
      domainMissing: "Domain missing"
    },

    placeholder: {
      reply: "Reply content goes here",
      largeThan: "Input a amount large than 0.01"
    },

    tooltip: {
      add: "Add",
      addRelativeSource: "Add relative source",
      addAnotherRuleAfterThis: "Add another rule after this",
      pageRouterManager: "Route Setting",
      uploadNewContent: "Import package",
      exportSite: "Export site",
      columnSetting: "Columns setting",
      moveUp: "Move up",
      moveDown: "Move down",
      copied: "Copied",
      image: {
        listView: "List view",
        gridView: "Grid view",
        zoomIn: "Zoom in",
        zoomOut: "Zoom out",
        moveLeft: "Move left",
        moveRight: "Move right",
        rotateLeft: "Rotate left",
        rotateRight: "Rotate right",
        horizontalFlip: "Horizontal flip",
        verticalFlip: "Vertical flip"
      },
      menu: {
        editTemplate: "Edit template",
        addSubMenu: "Add sub menu",
        multilingual: "Multilingual"
      },
      demand: {
        acceptProposal: "Accept this proposal"
      }
    },

    validationRule: {
      required: "Required",
      stringLength: "String length",
      regex: "Regex",
      email: "Email",
      dataType: "Data type",
      range: "Range",
      min: "Min",
      max: "Max",
      minLength: "Min string length",
      maxLength: "Max string length",
      stringLengthRange: "String length range",
      minChecked: "Min checked",
      maxChecked: "Max checked",
      regex: "Regex"
    },

    validate: {
      minimalValue: "Minimal value",
      maximumValue: "Maximum value",
      pattern: "Pattern",
      minimalChecked: "Minimal checked",
      maximumChecked: "Maximum checked",
      minimalLength: "Minimal length",
      maximumLength: "Maximum length",
      domainInvalid: "invalid domain"
    },

    action: {
      add: "Add",
      update: "Update",
      delete: "Delete"
    },

    component: {
      tableRelation: {
        tableA: "Table A",
        fieldA: "Field A",
        tableB: "Table B",
        fieldB: "Field B"
      },
      event: {
        noCondition: "No condition",
        noActivity: "No activity",
        addActivity: "Add activity",
        editActivity: "Edit activity"
      },
      progress: {
        uploading: "Uploading",
        uploaded: "Uploaded",
        pleaseWait: "please wait..."
      },
      breadCrumb: {
        sites: "Sites",
        dashboard: "Dashboard",
        market: "Market"
      },
      events: {
        editCondition: "Edit condition"
      },
      controlType: {
        textBox: "Text Box",
        textArea: "Text Area",
        wysiwygEditor: "WYSIWYG Editor",
        richEditor: "Rich Editor",
        checkBox: "Checkbox",
        mediaFile: "Media File",
        file: "File",
        selection: "Selection",
        dateTime: "Date Time",
        number: "Number",
        radioBox: "Radiobox",
        email: "Email",
        password: "Password",
        plainText: "Plain text",
        submitBtn: "Submit button",
        resetBtn: "Reset button",
        divider: "Divider",
        submitAndResetBtn: "Submit and reset button",
        switch: "Switch",
        int32: "Int32",
        boolean: "Boolean",
        tinymce: "Tinymce",
        dynamicSpec: "Dynamic specification",
        fixedSpec: "Fixed specification"
      },
      modal: {
        embeddedFolder: "Embedded Folder"
      },
      fieldEditor: {
        title: "Field Editor",
        field: "Field",
        createField: "Create Field",
        editField: "Edit Field",
        basic: "Basic",
        advanced: "Advanced",
        validation: "Validation",
        controlType: "Control type",
        summaryField: "Summary field",
        specificationField: "Specification field",
        enableMultipleSelection: "Enable multiple selection",
        enableMultiple: "Enable multiple",
        enableMultilingual: "Enable multilingual",
        selectValidationRules: "Select validation rules",
        tooltip: "Tooltip",
        Options: "Options",
        TextBox: "TextBox",
        TextArea: "TextArea",
        Tinymce: "Tinymce",
        RichEditor: "Rich Editor",
        CheckBox: "CheckBox",
        RadioBox: "RadioBox",
        MediaFile: "MediaFile",
        Selection: "Selection",
        DateTime: "DateTime",
        Number: "Number",
        Boolean: "Bool",
        errorMsg: "Error message",
        errorMsgHint: "Error message when validation failed",
        userEditable: "User editable"
      },
      fieldPanel: {
        categories: "Categories"
      },
      columnEditor: {
        title: "Column settings",
        primaryKey: "Primary key",
        index: "Index",
        unique: "Unique",
        incremental: "Incremental",
        seed: "Seed",
        scale: "Scale",
        unableToChangeSelfIncrementField:
          "Unable to change self-incremental field."
      },
      header: {
        user: {
          profile: "Profile",
          signOut: "Sign out"
        }
      },
      table: {
        page: "Page",
        script: "Script",
        style: "Style",
        layout: "Layout",
        label: "Label",
        view: "View",
        htmlblock: "HTML Block",
        cssrule: "CSS Rule",
        file: "File",
        route: "Route",
        relation: "Relation",
        image: "Image",
        textcontent: "Text Content",
        binding: "Binding",
        menu: "Menu",
        website: "Web Site",
        user: "User",
        contentfolder: "Content Folder",
        domain: "Domain",
        usergroup: "User Group",
        externalresource: "External Resource",
        thumbnail: "Thumbnail",
        folder: "Folder",
        cssdeclaration: "CSS Declaration",
        notification: "Notification",
        datamethodsetting: "Data Method Setting",
        link: "Link",
        embedded: "Embedded",
        form: "Form",
        syncsetting: "Sync Setting",
        contentcategory: "Content Category",
        contenttype: "Content Type",
        formvalue: "Form Value",
        viewdatamethod: "View Data Method",
        domelement: "Dom Element",
        unknown: "Unknown",
        kooboosystem: "Kooboo System",
        resourcegroup: "Resource Group",
        synchronization: "Synchronization",
        disksync: "Disk Sync",
        api: "API",
        url: "URL"
      },
      sidebar: {
        currentSite: "current site"
      },
      htmlViewer: {
        code: "Code"
      },
      multilingualselector: {
        default: "(Default)"
      },
      pageEditor: {
        basic: "Basic",
        contentTitle: "Content title",
        basicHelper: "The value for page html title tag",
        URL: "URL",
        urlHelper: "Customize the page URL",
        htmlMeta: "HTML meta",
        content: "Content",
        parameters: "Parameters",
        invalidRoute: "You have input a invalid url route, please check"
      },
      viewEditor: {
        selectHint: "-- Select field --",
        attribute: "Attribute",
        condition: "Condition",
        type: "Type",
        normal: "Normal",
        repeat: "Repeat",
        repeatCondition: {
          odd: "Odd",
          even: "Even",
          first: "First",
          last: "Last",
          number: "Number"
        },
        data: "Data",
        bindTo: "Bind to",
        openInNewWindow: "Open in new window",
        link: "Link",
        linkTo: "Link to",
        parameter: "Parameter",
        value: "Value",
        repeatedItem: "Repeated item",
        repeatSelf: "Repeat self",
        dynamicParam: "Dynamic parameters",
        currentPage: "Current page"
      },
      layoutEditor: {
        position: "Position",
        replaceOuterTag: "Replace outer tag"
      },
      styleScript: {
        files: "Files",
        groups: "Groups"
      },
      actionDialog: {
        parameter: "Parameter",
        mapTo: "Map to",
        aliasName: "Alias name",
        global: "Global",
        local: "Local"
      },
      folderEditor: {
        folder: "Folder",
        basicInfo: "Basic Info",
        relationFolders: "Relation Folders",
        displayName: "Display name",
        categoryFolders: "Category folders",
        multiple: "Multiple",
        EmbeddedFolders: "Embedded folders",
        chooseItemBelow: "Please select items shown below",
        noFolderAvailable: "No folder available",
        alias: "Alias"
      },
      styleEditor: {
        pickImage: "Pick image",
        declaration: "Declaration",
        value: "Value",
        showRelation: "Show relation",
        pickStyle: "Pick style",
        addProperty: "Add Property",
        removeSelector: "Remove this selector"
      },
      exportModal: {
        title: "Export",
        exportType: "Export type",
        exportContent: "Export content",
        complete: "Complete",
        custom: "Custom"
      },
      conditionModal: {
        title: "Conditions",
        condition: "Condition",
        add: "Add condition"
      },
      hardwareModal: {
        quantity: "Quantity",
        selectTypeFirst: "Select type first",
        startMonth: "Start month"
      },
      cashierModal: {
        choosePaymentMethod: "Please choose a payment method"
      },
      topupModal: {
        title: "Topup",
        history: "Topup history",
        paymentMethod: "Payment method",
        rechargeAmount: "Recharge amount",
        customize: "Customize",
        largeThan: "large than 0.01",
        paid: "paid",
        orderDate: "Order date",
        coupon: "coupon"
      },
      templateModal: {
        downloads: "Downloads",
        dynamicContent: "Dynamic contents",
        hint: "Use the selected template as the base to create this website",
        siteName: "Site name",
        siteNameHint: "Start with letters and no space allowed",
        domainHint: "Give your site a domain that other people can access.",
        useThisTemplate: "Use this template",
        subDomain: "SubDomain"
      },
      demandModal: {
        publishDemand: "Publish a demand",
        requireSkill: "Required skills",
        startTime: "Start time",
        endTime: "End time"
      },
      discussionModal: {
        addDiscussion: "Add a discussion"
      },
      expertiseModal: {
        expertise: "Expertise"
      },
      objectionModal: {
        objection: "Objection",
        receiveObjection: "We have receive your objection",
        contact: "Contact"
      },
      orderModal: {
        supplier: "Supplier:",
        order: "Buy it"
      },
      proposalModal: {
        proposal: "Proposal",
        duration: "Duration",
        days: "Day(s)",
        bidder: "Bidder",
        accept: "Accept",
        day: "Day",
        s: "s"
      },
      dataCenterModal: {
        changeExplanation:
          "Change the datacenter only when the assigned datacenter is far from your primary location. You need to backup and restore your site data manually."
      },
      userVerifyModal: {
        verify: "Verify"
      }
    },
    site: {
      sites: {
        takeOffline: "Take offline",
        takeOnline: "Take online",
        transferAWebsiteTip:
          "Enter any website Url, Kooboo will clone the website and allow you to inline edit anything directly.",
        templatesTip:
          "Share or use templates made by public users or from your organization private packages.",
        createTip:
          "Create a blank new website, and use well defined elements to develop your website.",
        importTip:
          "Import a Kooboo or standard Html package, Kooboo will convert it into a Kooboo site.",
        createModalTip:
          "Create a blank new website, and use well defined elements to develop your website.",
        onlineVersionCloneTip:
          "* This online version allows you to clone 3 pages with very limited speed, use desktop version if you need more.",
        automatically:
          "System will add thumbnails automatically if user don't upload any thumbnail.",
        shareTip:
          "Share your website as a template for public users or within your organization."
      },
      setting: {
        configuration: "Configuration"
      },
      sync: {
        listName: "List",
        remoteSite: "Remote site",
        server: "Server",
        difference: "Difference",
        localChanges: "Local changes",
        pullLog: "Pull log",
        pushLog: "Push log",
        pushType: {
          all: "Push all",
          update: "Push update only"
        }
      },
      images: {
        all: "All",
        page: "Page",
        style: "Style",
        view: "View",
        layout: "Layout",
        content: "Content",
        free: "Free"
      },
      page: {
        createNewLayout: "Click to create your first layout",
        design: "Inline edit",
        linked: "Linked",
        online: "Online",
        references: "References",
        systemDefault: "System default",
        position: "Position",
        copyPage: "Copy page",
        fromLayout: "From layout",
        previewInMobile: "Preview in mobile",
        redirectRoutesTip:
          "Set the redirect pages for default home page, 404 and error pages"
      },
      layout: {
        copyLayout: "Copy layout"
      },
      share: {
        article: "article",
        corporation: "corporation",
        category: "category"
      },
      siteLog: {
        logItem: "Log item",
        action: "Action",
        type: "Type",
        version: "Version",
        currentVersion: "Current version"
      },
      siteUser: {
        role: "Role"
      },
      visitorLog: {
        all: "All",
        topPages: "Top pages",
        topReferer: "Top referer",
        topImages: "Top Images",
        errorList: "Error List",
        chart: "Chart",
        ip: "IP",
        page: "Page",
        startTime: "Start time",
        timeElapsed: "Time elapsed",
        referer: "Referer",
        views: "Views",
        errorCount: "Error count",
        country: "Country",
        state: "State"
      },
      disk: {
        count: "Count"
      },
      job: {
        startTime: "Start at",
        repeat: "Repeat",
        pending: "Pending",
        completed: "Completed",
        jobName: "Job name",
        executionTime: "Execution time",
        message: "Message"
      },
      diagnostic: {
        information: "Information",
        warning: "Warning",
        critical: "Critical",
        scanAgain: "Scan again",
        scanning: "scanning!"
      },
      contentType: {
        fields: "Fields",
        showSystemField: "Click to show/hide system fields",
        systemFieldUndeletable: "You cann't remove system field.",
        editContentType: "Edit content type"
      },
      textContent: {
        new: "New",
        changeContentTypeConfirm:
          "Current data will not be saved if you change type. Would you want to continue?",
        createTypeFieldHint:
          "You don't have any content type yet. Please create a content type before you save.",
        saveWidthNoFieldHint:
          "You don't have any field yet. Use 'New field' button to create your field.",
        createFolderHint:
          "You dont't have folder yet. Use 'New folder' button to create a folder first."
      },
      view: {
        dataSource: "Data Source",
        configureDataSource: "Configure datasource",
        copyView: "Copy view",
        dummy: "Dummy",
        current: "Current"
      },
      dataSource: {
        methodName: "Method Name"
      },
      form: {
        refreshSelf: "Refresh self",
        globalStyle: "Global style",
        submit: "Submit",
        reset: "Reset"
      },
      script: {
        children: "Children",
        external: "External",
        embedded: "Embedded",
        group: "Group",
        groupName: "Group name"
      },
      style: {
        name: "Style",
        ownerType: "Owner type",
        siteObject: "Site object",
        inline: "Inline style"
      },
      URL: {
        name: "URLs",
        internal: "Internal",
        external: "External",
        resourceType: "Resource type",
        hasObject: "Has object"
      },
      label: {
        key: "Key",
        value: "Value",
        originalValue: "Original value",
        translatedValue: "Translated value",
        createANewLabel: "Create a new label",
        placeholder: "New label"
      },
      htmlblock: {
        readOnly: "Read only"
      },
      domain: {
        name: "Domain",
        domains: "Domains",
        site: "Site",
        years: "years",
        year: "year",
        email: "Email",
        records: "Records",
        expires: "Expires",
        sslEnabled: "SSL enabled"
      },
      template: {
        public: "Public",
        personal: "Personal",
        noDescription: "No description",
        noTag: "No tag",
        noDynamic: "No dynamic content",
        weHaveFound: "We have found ",
        templateUnit: " template(s)"
      },
      profile: {
        Account: "Account",
        Password: "Password",
        Organization: "Organization",
        Users: "Users",
        DataCenter: "Data center"
      },
      search: {
        searchedCount: "Searched count",
        keywords: "Keywords",
        docFound: "Document found"
      },

      multiLang: {
        noTranslation: "No translation",
        translationState: "Translation state"
      },
      code: {
        codeType: "Code type",
        chooseCodeType: "Choose code type",
        noSetting: "No setting",
        noActivityAndCreate: "No activity available! Click here to create one."
      }
    },

    market: {
      Used: "Used: ",
      Total: "Total: ",
      changeCurrency: "Change currency",
      demand: {
        myDemands: "My Demands",
        myProposals: "My Proposals",
        StartDate: "Start date",
        EndDate: "End date",
        demander: "Demander",
        createTime: "Create time",
        isOpen: "Is open",
        proposalCount: "proposalCount"
      },
      discussion: {
        myThreads: "My Threads",
        Comments: "Comments"
      },
      hardware: {
        usageReport: "Usage Report"
      },
      supplier: {
        myOffers: "My Offers",
        myOrders: "My Orders",
        myServices: "My Services",
        service: "Service",
        status: "Status",
        orderUser: "Order user",
        me: "Me"
      },
      template: {
        myTemplate: "My Template",
        privateTemplate: "Private Template"
      }
    },

    mail: {
      Inbox: "Inbox",
      Sent: "Sent",
      Spam: "Spam",
      read: "Read",
      unread: "Unread",
      to: "Forward to",
      moveTo: "Move to",
      jumpToInbox: "Jump to inbox",
      markAsRead: "Mark as read",
      markAsUnread: "Mark as unread",
      noSubject: "No subject",
      address: {
        name: "Address",
        normal: "Normal",
        wildcard: "Wildcard",
        groupMail: "Group mail",
        forwarding: "Forwarding"
      },
      noAddressYet:
        "You don't have a address yet, please add a new address before composing a email.",
      attachments: " Attachment(s)",
      noAddressFound: "No address found",
      wildcardTip:
        "Use * to represent any characters. * is required and only one * is allowed"
    },

    commerce: {
      isEnabled: "E-commerce enabled",
      product: {
        inStock: "In stock",
        dropOff: "Drop off"
      }
    },

    logical: {
      and: "Logical and"
    },

    dashboard: {
      noPageViewed: "No page has been viewed yet.",
      total: "Total",
      IPs: "IPs",
      Pages: "Pages",
      avgSize: "Avg Size"
    },

    popover: {
      searchStatusExplain: "Add to index automatically when documnent changed."
    },

    inlineEditor: {
      pageLink: "Page link",
      outLink: "Out link",
      copyBlock: "Copy this block",
      Copy: "Copy",
      editImage: "Edit image",
      editLink: "Edit link",
      editImages: "Edit Images",
      editLinks: "Edit Links",
      editData: "Edit data",
      editHtml: "Edit html",
      editHtmlblock: "Edit htmlblock",
      enterLink: "Enter link",
      editMenu: "Edit menu",
      editStyle: "Edit style",
      editRepeaterItem: "Edit repeater item",
      width: "Width",
      height: "Height",
      editAfter: "Please edit after save",
      copyRepeaterItem: "Copy repeater item",
      removeRepeaterItem: "Remove repeater item",
      replacewithImage: "Replace with image",
      replacewithText: "Replace with text",
      move: "Move",
      undo: "Undo",
      redo: "Redo",
      alt: "Alt",
      contentImages: "Content Images",
      styleImages: "Style Images",
      domImages: "DOM Images",
      contentLinks: "Content links",
      domLinks: "DOM links",
      backgroundImage: "Background-image",
      backgroundColor: "Background-color",
      color: "Color",
      font: "Font",
      fontFamily: "Font-Family",
      fontSize: "Font-Size",
      fontWeight: "Font-Weight",
      click: "Click",
      expandSelection: "Expand selection",
      globalUpdate: "Global update",
      editColor: "Edit color",
      htmlModeTip:
        "Failed to initialize the editor as the document is not in standards mode",
      clickReplace: "Click to replace"
    }
  };
  Kooboo.text = text;
})();
