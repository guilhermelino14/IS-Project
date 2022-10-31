## Application Routes
* POST
  ```sh
  /api/somiod 

        -> [FromBody] Application
  ```
* GET
  ```sh
  /api/somiod 
  ```
* PUT
  ```sh
  /api/somiod/{id:int}

        -> int id
        -> [FromBody] Application
  ```

* DELETE
  ```sh
  /api/somiod/{id:int}

        -> int id
  ```       



## Module Routes
* POST
  ```sh
  /api/somiod/{module:alpha}

        -> [FromBody] Model
  ```
* GET
  ```sh
  /api/somiod/{module:alpha}
  ```
* PUT
  ```sh
  /api/somiod/{module:alpha}/{id:int}

        -> int id
        -> [FromBody] Model
  ```

* DELETE
  ```sh
  /api/somiod/{module:alpha}/{id:int}

        -> int id
  ```   


## Module Routes
* POST
  ```sh
  /api/{module:alpha}/{value:alpha}

        -> [FromBody] ...
  ```
* GET
  ```sh
  /api/{module:alpha}/{value:alpha}
  ```
* PUT
  ```sh
  /api/somiod/{module:alpha}/{value:alpha}/{id:int}

        -> int id
        -> [FromBody] ...
  ```

* DELETE
  ```sh
  /api/somiod/{module:alpha}/{value:alpha}/{id:int}

        -> int id
  ```   

        

