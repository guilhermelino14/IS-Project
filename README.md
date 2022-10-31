Application Routes
POST -> /api/somiod
        -> [FromBody] Application
GET -> /api/somiod
PUT -> /api/somiod/{id:int}
        -> int id
        -> [FromBody] Application
DELETE -> /api/somiod/{id:int}
        -> int id


Module Routes
POST -> /api/somiod/{module:alpha}
        -> [FromBody] Model
GET -> /api/somiod/{module:alpha}
PUT -> /api/somiod/{module:alpha}/{id:int}
        -> int id
        -> [FromBody] Model
DELETE -> /api/somiod/{module:alpha}/{id:int}
        -> int id


SubModule Routes
POST -> /api/{module:alpha}/{value:alpha}
        -> [FromBody] ...
GET -> /api/{module:alpha}/{value:alpha}
PUT -> /api/somiod/{module:alpha}/{value:alpha}/{id:int}
        -> int id
        -> [FromBody] ...
DELETE -> /api/somiod/{module:alpha}/{value:alpha}/{id:int}
        -> int id
