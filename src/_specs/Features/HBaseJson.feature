Feature: HBase JSON
	In order to work with HBase content in the JSON format
	As an application developer
	I want to be able to create and parse HBase JSON content using basic information

Background: 
	Given I have everything I need to test a content converter for JSON

Scenario Outline: Create JSON for single cell
	Given I have a cell with a <row>, <column>, <qualifier>, <timestamp>, and <value>
	When I convert my cell to raw content
	Then my raw JSON content should be equivalent to the resource called "<expected json>"
Examples:
	| row | column | qualifier | timestamp | value       | expected json                 |
	| 1   | alpha  |           |           | hello world | HBaseJson_1Alpha_HelloWorld   |
	| 1   | alpha  | x         |           | hello world | HBaseJson_1AlphaX_HelloWorld  |
	| 1   | alpha  |           | 4         | hello world | HBaseJson_1Alpha4_HelloWorld  |
	| 1   | alpha  | x         | 4         | hello world | HBaseJson_1AlphaX4_HelloWorld |

Scenario Outline: Parse JSON for single cell
	Given I have raw content equal to the resource called "<initial json>"
	When I convert my raw content to a cell
	Then my cell should have a <row>, <column>, <qualifier>, <timestamp>, and <value>
Examples:
	| initial json                  | row | column | qualifier | timestamp | value       |
	| HBaseJson_1Alpha_HelloWorld   | 1   | alpha  |           |           | hello world |
	| HBaseJson_1AlphaX_HelloWorld  | 1   | alpha  | x         |           | hello world |
	| HBaseJson_1Alpha4_HelloWorld  | 1   | alpha  |           | 4         | hello world |
	| HBaseJson_1AlphaX4_HelloWorld | 1   | alpha  | x         | 4         | hello world |

Scenario: Create JSON for a set of cells
	Given I have created a set of cells
	And I have added a cell to my set with the following properties:
		| row | column | qualifier | timestamp | value       |
		| 1   | alpha  |           |           | hello world |
		| 1   | alpha  | x         |           | hello world |
		| 1   | alpha  |           | 4         | hello world |
		| 1   | alpha  | x         | 4         | hello world |
	When I convert my set of cells to raw content
	Then my raw JSON content should be equivalent to the resource called "HBaseJson_Set_HelloWorld"

Scenario: Parse JSON for a set of cells
	Given I have raw content equal to the resource called "HBaseJson_Set_HelloWorld"
	When I convert my raw content to a set of cells
	Then my set should contain 4 cells
	And one of the cells in my set should have the following properties:
		| row | column | qualifier | timestamp | value       |
		| 1   | alpha  |           |           | hello world |
		| 1   | alpha  | x         |           | hello world |
		| 1   | alpha  |           | 4         | hello world |
		| 1   | alpha  | x         | 4         | hello world |