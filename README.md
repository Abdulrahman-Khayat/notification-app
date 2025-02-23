# Notification App
A simple application that implements User Nofification Service 



## System Design 
system design is built by following <a href="https://c4model.com/" target="_blank">C4 Models</a> using 
<a href="https://structurizr.com/" target="_blank">Structurizr</a>

to run Structurizr, run the following command in the home directory of the project:

        docker run -it -d --rm -p 7601:8080 -v ./:/usr/local/structurizr structurizr/lite
        

![alt text](image.png) 

## How to Run (for development)
1. Download the code
2. In the project directory run docker to start the required services:

        docker compose up -d

3. Run each project using:

        dotnet run

## 