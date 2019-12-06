/*
 *
 * This function is kb-code-editor`s auto-complete for kscript
 *
 */

interface Kscript {
  /**
   * Access to the http request data, query string, form or headers. Cookie is available from k.cookie.
   */
  request: {
    /**
     * The query string collection
     */
    queryString: Dictionary;
    /**
     * form
     */
    form: Dictionary;
    /**
     * method
     */
    method: string;
    /**
     * clientIp
     */
    clientIp: string;
    /**
     * headers
     */
    headers: any;
    /**
     * url
     */
    url: string;
    /**
     * 	A collection of UploadFile
     */
    files: UploadFile[];
  };

  /**
   * The http response object that is used to set data into http resposne stream
   */
  response: {
    /**
     * Excute another Url, and write the response within current context
     * @param Url
     */
    execute(Url: string);
    /**
     * Print the object in Json format, if the object is a value type like string, or number, it will print the string format of that value.
     * @param value
     */
    json(value: object);

    /**
     * Redirect user to another url, url can be relative or absolute, status code 302
     * @param url
     */
    redirect(url: string);
    /**
     * set header value on output html page.
     * @param key
     * @param value
     */
    setHeader(key: string, value: string);
    /**
     * 	Print the input on output page. If it is not a value type object, it will print Json format of that object.
     * @param value
     */
    write(value: object);
  };

  session: {
    /**
     * remove all items from session
     */
    clear();
    /**
     * check whether session has the key or not.
     * @param key
     */
    contains(key: string): boolean;
    /**
     * get stored session value
     * @param key
     */
    get(key: string);
    /**
     * Remove item from session by session key.
     * @param key
     */
    remove(key: string);
    /**
     * Set a Key Value in the session store.
     * @param key
     * @param value
     */
    set(key: string, value: object);
    key: any;
    keys: [];
    values: [];
  };

  /**
   * Get or set cookie value
   */
  cookie: {
    keys: [];
    values: [];
    length: number;
    item: string;
    /**
     * remove all items from cookie
     */
    clear();
    /**
     * 	check whether cookie has the key or not.
     * @param key
     */
    containsKey(key: string);
    /**
     * Get the cookie value by name
     * @param name
     */
    get(name: string);
    /**
     * Remove item from cookie by session key.
     * @param key
     */
    remove(key: string);
    /**
     * 	set a cookie with defined expiration days
     * @param name
     * @param value
     * @param days
     */
    set(name: string, value: string, days: number);
    /**
     * set a cookie that expires in 1 day.
     * @param name
     * @param value
     */
    set(name: string, value: string);
    /**
     * 	set the cookie with an expiration time in minutes.
     * @param name
     * @param value
     * @param mins
     */
    setByMinutes(name: string, value: string, mins: number);
  };

  /**
   * The Kooboo website database with version control
   */
  site: {
    pages: RoutableTextRepository;
    views: TextRepository;
    layouts: TextRepository;
    textContents: TextContentRepository;
    htmlBlocks: MultilingualRepository;
    labels: MultilingualRepository;
    scripts: RoutableTextRepository;
    styles: RoutableTextRepository;
    images: BinaryRepository;
    files: BinaryRepository;
  };

  /**
   * Get content from or post data to remote url.
   */
  url: {
    /**
     * Get data string from the url
     * @param url
     */
    get(url: string): string;
    /**
     * get data string from remote url by using HTTP Basic authentication
     * @param url
     * @param username
     * @param password
     */
    get(url: string, username: string, password: string): string;
    /**
     * 	Post data to remote url
     * @param url
     * @param data
     */
    post(url: string, data: string): string;
    /**
     * Post data to remote url using HTTP basic authentication
     * @param url
     * @param data
     * @param userName
     * @param password
     */
    post(url: string, data: string, userName: string, password: string): string;
    /**
     * Post data as JSON to remote url using HTTP basic authentication
     * @param url
     * @param data
     * @param userName
     * @param password
     */
    postData(
      url: string,
      data: object,
      userName: string,
      password: string
    ): string;
  };

  /**
   * Provide read and write access to text or binary files under the site folder. Below is fully functioning example.
   */
  file: {
    /**
     * Write the text to the file name. When the target is NOT exist, it will be created
     * @param fileName
     * @param content
     */
    append(fileName: string, content: string): void;
    /**
     * create a sub folder under current folder
     * @param newFolderName
     * @param parentFolder
     */
    createFolder(newFolderName: string, parentFolder: string): void;
    /**
     * 	Delete the file
     * @param fileName
     */
    delete(fileName: string): void;
    /**
     * Delete a file folder includes all sub directories and files.
     * @param folder
     */
    deleteFolder(folder: string): boolean;
    /**
     * Delete a folder and all sub directories and files in it.
     * @param folderName
     */
    deleteFolder(folderName: string);
    /**
     * 	Check whether the file exists or not, filename can be: /folder/filename.txt.
     * @param fileName
     */
    exists(fileName: string): boolean;
    /**
     * Return all files in the provided folder, return an Array of FileInfo
     * @param folder
     */
    folderFiles(folder: string);
    /**
     * 	Return all files in all folders, return an Array of FileInfo
     */
    getAllFiles();
    /**
     * Read all the text of the file
     * @param fileName
     */
    read(fileName: string);
    /**
     * read the file into a byte array
     * @param fileName
     */
    readBinary(fileName: string): [];
    /**
     * List sub folders under current folder, return an Array of FolderInfo
     * @param folder
     */
    subFolders(folder: string): FolderInfo;
    /**
     * 	Write the text to the file name. When the target is exist, it will be overwritten.
     * @param fileName
     * @param content
     */
    write(fileName: string, content: string): FileInfo;
    /**
     * Write an array of bytes to the site disk folder.
     * @param fileName
     * @param binary
     */
    writeBinary(fileName: string, binary: []): FileInfo;
  };
  /**
   * Kooboo KeyValue store per site. Both key and value are string only.
   */
  keyValue: {
    keys: [];
    values: [];
    length: number;
  };
  /**
   * database
   */
  database: {
    /**
     * Return the kScript database table object, if the table is not exists, it will be created.
     * @param name
     */
    getTable(name: string): Table;
  };

  /**
   * the dataContext of kview engine, the html render engine of kooboo. You can explicitly set value into datacontext or just declare the value as JS global variable, it will be accesible from kview engine as well.
   */
  dataContext: {
    /**
     * get existing object from dataContext
     * @param key
     */
    get(key: string): object;
    /**
     * 	set function
     * @param key
     * @param value
     */
    set(key: string, value: object): void;
  };

  mail: {
    /**
     * Send an email from current host. You may need credit to send internet emails on online version
     * @param Msg
     */
    send(Msg: object): void;
    /**
     * Send an email using an external smtp server
     * @param Server
     * @param Msg
     */
    smtpSend(Server: SmtpServer, Msg: object): void;
  };

  /**
   * Private MD5 and SHA1 encryption and a simple two way encrypt and decrypt function
   */
  security: {
    /**
     * Encrypt a string based on the key.
     * @param Value
     * @param key
     */
    decrypt(Value: string, key: string): string;
    /**
     * Encrypt a string based on the key.
     * @param Input
     * @param key
     */
    encrypt(Input: string, key: string): string;
    /**
     * Compute a 32 length text string value
     * @param Input
     */
    md5(Input: string): string;
    /**
     * Compute a 40 length text string value
     * @param Input
     */
    sha1(Input: string): string;
  };

  /**
   * Access to current request information.
   */
  Info: {
    /**
     * The name of current website
     */
    name: string;
    /**
     * Current request culture
     */
    culture: string;
    /**
     * The website custom setting. Example: k.info.setting.mykey;
     */
    setting: string;
    /**
     * the login user information if any, aslo available under k.user. Fields are: username, firstname, lastname, language.
     */
    user: User;
  };

  user: User;
}

interface RoutableTextRepository {
  /**
   * add an item
   * @param object
   */
  add(object: object);
  /**
   * Return an array of the SiteObjects
   */
  all(): [];
  /**
   * Delete an item
   * @param nameOrId
   */
  delete(nameOrId: object);
  /**
   * Get an item based on Name or Id
   * @param nameOrId
   */
  get(nameOrId: object): object;
  /**
   * get the absolute Url of this object
   * @param id
   */
  getAbsUrl(id: object): object;
  /**
   * get the SiteObject by Url
   * @param url
   */
  getByUrl(url: string): object;
  /**
   * get the relative Url of this object
   * @param id
   */
  getUrl(id: object): object;
  /**
   * update the text object
   * @param siteObject
   */
  update(siteObject: Object);
  /**
   * Update the text value of the body property
   * @param nameOrId
   * @param newbody
   */
  updateBody(nameOrId: object, newbody: string);
}

interface TextRepository {
  /**
   * add an item
   * @param object
   */
  add(object: object);
  /**
   * Return an array of the SiteObjects
   */
  all(): [];
  /**
   * Delete an item
   * @param nameOrId
   */
  delete(nameOrId: object);
  /**
   * 	Get an item based on Name or Id
   * @param nameOrId
   */
  get(nameOrId: object): object;
  /**
   * update the text object
   * @param siteObject
   */
  update(siteObject: object);
  /**
   * Update the text value of the body property
   * @param nameOrId
   * @param newbody
   */
  updateBody(nameOrId: object, newbody: string);
}

interface TextContentRepository {
  /**
   * Add a text content into content repository. Folder is a required.
   * @param value
   */
  add(value: object);
  /**
   * Return an array of all TextContentObjects
   */
  all(): TextContentObject;
  /**
   * Delete an item based on id or userkey
   * @param id
   */
  delete(id: string);
  /**
   * Return the first TextContentObject based on search condition
   * @param QuerySearchCondition
   */
  find(QuerySearchCondition: string): TextContentObject;
  /**
   * Return an array of all matched TextContentObjects
   * @param QuerySearchCondition
   */
  findAll(QuerySearchCondition: string): TextContentObject[];
  /**
   * Get a text content object based on Id or UserKey
   * @param nameorid
   */
  get(nameorid: string): TextContentObject;
  /**
   * Return the first TextContentObject based on search condition
   * @param QuerySearchCondition
   */
  query(QuerySearchCondition: string): ContentQuery;
  /**
   * update a text content values
   * @param value
   */
  update(value: TextContentObject);
}

interface MultilingualRepository {
  /**
   * add an item
   * @param object
   */
  add(object: MultilingualObject);
  /**
   * Return an array of the SiteObjects
   */
  all(): MultilingualObject[];
  /**
   * Delete an item
   * @param nameOrId
   */
  delete(nameOrId: object);
  /**
   * Get an item based on Name or Id
   * @param object
   * @param nameOrId
   */
  get(nameOrId: object): MultilingualObject;
  /**
   * update the text value
   * @param value
   */
  update(value: MultilingualObject);
}
interface BinaryRepository {
  /**
   * Return an array of the SiteObjects
   */
  all(): [];
  /**
   * Delete an item
   * @param nameOrId
   */
  delete(nameOrId: object);
  /**
   * Get an item based on the unique Guid
   * @param nameOrId
   */
  get(nameOrId: object): object;
  /**
   * update the text object
   * @param siteObject
   */
  update(siteObject: object);
  /**
   * Update the binary content
   * @param nameOrId
   * @param newbody
   */
  updateBinary(nameOrId: object, newbody: string);
}

interface MultilingualObject {
  /**
   * Get or set the value of current culture
   */
  value: string;
}

interface ContentQuery {
  /**
   * the number of items that will be skipped
   */
  skipcount: number;
  /**
   * Is ascending order
   */
  ascending: boolean;
  /**
   * The field name to order by
   */
  orderByField: string;
  /**
   * The search condition text
   */
  searchCondition: string;

  count(): number;
  /**
   * the order by field name
   * @param fieldname
   */
  orderBy(fieldname: string): tableQuery;
  /**
   * the field name to order by in Descending order
   * @param fieldname
   */
  orderByDescending(fieldname: string): tableQuery;
  skip(skip: number): tableQuery;
  take(count: number): [];
}

interface TextContentObject {
  /**
   * set to read content properties based on a different culture
   * @param culture
   */
  setCulture(culture: string): TextContentObject;
}

interface UploadFile {
  /**
   * ContentType
   */
  contentType: string;
  /**
   * FileName
   */
  fileName: string;
  /**
   * The binary bytes array
   */
  bytes: [];
  /**
   * save the file into disk
   * @param string
   * @param filename
   */
  save(filename: string);
}

interface Dictionary {
  keys: object;
  values: object;
  length: number;
  item: string;
  /**
   * add new value into the collection
   * @param key
   * @param value
   */
  add(key: string, value: string);
  /**
   * check whether the collection has that key or not.
   * @param key
   */
  contains(key: object): boolean;
  /**
   * get value form the collection.
   * @param string
   * @param key
   */
  get(key: string): string;
}

interface FileInfo {
  /**
   * File name
   */
  name: string;
  /**
   * Full relative File name includes folders
   */
  fullName: string;
  /**
   * the byte length
   */
  size: number;
  /**
   * Relative url
   */
  relativeUrl: string;
  /**
   * Same as relative url
   */
  url: string;
  /**
   * AbsoluteUrl includes http and website domain
   */
  absoluteUrl: string;
  /**
   * Last modified time
   */
  lastModified: Date;
}

interface FolderInfo {
  /**
   * File name
   */
  name: string;
  /**
   * Full relative File name includes folders
   */
  fullName: string;
}

interface Table {
  /**
   * Add an object into database table. If the table does not have the field, the table schema will be automatically updated with that column.
   * @param value
   */
  add(value: object): object;
  all(): [];
  /**
   * 	Append an object to database table. This is the same as "add" except that it will not update table schema when the object contains fields not defined in the table schema.
   * @param value
   */
  append(value: object): object;
  /**
   * Create an additional index.
   * @param fieldname
   */
  createIndex(fieldname: string): void;
  /**
   * Delete an item from database based on id or primary key
   * @param id
   */
  delete(id: object);

  /**
   * find the first item that field equal the match value
   * @param field
   * @param value
   */
  find(field: string, value: object): object;
  /**
   * Return the first item that matches the condition
   * @param QuerySearchCondition
   */
  find(QuerySearchCondition: string): object;
  /**
   * Return the all items that matches the condition
   * @param field
   * @param value
   */
  findAll(field: string, value: object): [];
  /**
   * Return the all items that matches the condition
   * @param QueryCondition
   */
  findAll(QueryCondition: string): [];
  /**
   * get an item based on Id or primary key
   * @param id
   */
  get(id: object);
  /**
   * Query table data with paging.
   * @param searchCondition
   */
  query(searchCondition: string): tableQuery;
  /**
   * update an item, key must be the system default Guid key or the key value of primary key field. you can set one field as the primary key.
   * @param id
   * @param newvalue
   */
  update(id: object, newvalue: object);
}

interface tableQuery {
  /**
   * the number of items that will be skipped
   */
  skipcount: number;
  /**
   * Is ascending order
   */
  ascending: boolean;
  /**
   * The field name to order by
   */
  orderByField: string;
  /**
   * The search condition text
   */
  searchCondition: string;

  count(): number;
  /**
   * the order by field name
   * @param fieldname
   */
  orderBy(fieldname: string): tableQuery;
  /**
   * the field name to order by in Descending order
   * @param fieldname
   */
  orderByDescending(fieldname: string): tableQuery;
  skip(skip: number): tableQuery;
  take(count: number): [];
  /**
   * The seach condition text
   * @param string
   * @param searchCondition
   */
  where(searchCondition: string): tableQuery;
}

interface User {
  /**
   * user name of current kooboo login user.
   */
  userName: string;
  /**
   * user id
   */
  id: string;
  /**
   * user first name
   */
  firstName: string;
  /**
   * user last name
   */
  lastName: string;
  /**
   * user language
   */
  language: string;
  /**
   * where there is a login user or not
   */
  isLogin: boolean;
  /**
   * Validate a kooboo user login information and return the user info if succeed
   * @param userName
   * @param password
   */
  login(userName: string, password: string): void;
  /**
   * logout current login user
   */
  logout(): void;
}

declare const k: Kscript;
