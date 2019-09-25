# cocapiprototype (WIP)

.Net Core 2.2 (have 2.2 sdk installed)

Right now we have two endpoints a 
GET that determines the questions to return from the dates and carrier passed in
    and 
POST that takes a CocRequest object that includes a List<< Question >> AnswerList from the body 
    and returns a CocResult object with the results of the scoring
    
Example GET

https://localhost:44324/api/coc?ProgramStartDate=2021-01-01T06:00:00.000Z&EntryDate=2021-07-10T05:00:00.000Z&DateOfService=2021-07-11T05:00:00.000Z&Client=MA

Example POST

https://localhost:44324/api/coc

with this body

{
	"somenum": 195,
	"somestr": "This is a test of the emergency broadcast network",
	"AnswerList": [
		{
			"id": 400,
			"quexText": "Reword this Changes to the member's treatment, including  additions, removals, or changes in administration of the drugs included in the regimen are not being submitted on this request.",
			"level": "2",
			"quexId": "2"
		},
		{
			"id": 600,
			"quexText": "Interruption of the services could potentially alter the progression of the condition or disease",
			"level": "3",
			"quexId": "4"
		}
	]
}
