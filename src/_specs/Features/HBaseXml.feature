Feature: HBase XML
	In order to work with HBase content in the XML format
	As an application developer
	I want to be able to create and parse HBase XML content using basic information

Background: 
	Given I have everything I need to test a content converter for XML

Scenario Outline: Create XML for single cell
	Given I have a cell with a <row>, <column>, <qualifier>, <timestamp>, and <value>
	When I convert my cell to raw content
	Then my raw XML content should be equivalent to the resource called "<expected xml>"
Examples:
	| row | column | qualifier | timestamp | value       | expected xml                 |
	| 1   | alpha  |           |           | hello world | HBaseXml_1Alpha_HelloWorld   |
	| 1   | alpha  | x         |           | hello world | HBaseXml_1AlphaX_HelloWorld  |
	| 1   | alpha  |           | 4         | hello world | HBaseXml_1Alpha4_HelloWorld  |
	| 1   | alpha  | x         | 4         | hello world | HBaseXml_1AlphaX4_HelloWorld |

Scenario Outline: Parse XML for single cell
	Given I have raw content equal to the resource called "<initial xml>"
	When I convert my raw content to a cell
	Then my cell should have a <row>, <column>, <qualifier>, <timestamp>, and <value>
Examples:
	| initial xml                  | row | column | qualifier | timestamp | value       |
	| HBaseXml_1Alpha_HelloWorld   | 1   | alpha  |           |           | hello world |
	| HBaseXml_1AlphaX_HelloWorld  | 1   | alpha  | x         |           | hello world |
	| HBaseXml_1Alpha4_HelloWorld  | 1   | alpha  |           | 4         | hello world |
	| HBaseXml_1AlphaX4_HelloWorld | 1   | alpha  | x         | 4         | hello world |

Scenario: Create XML for a set of cells
	Given I have created a set of cells
	And I have added cells to my set with the following properties:
		| row | column | qualifier | timestamp | value       |
		| 1   | alpha  |           |           | hello world |
		| 1   | alpha  | x         |           | hello world |
		| 1   | alpha  |           | 4         | hello world |
		| 1   | alpha  | x         | 4         | hello world |
	When I convert my set of cells to raw content
	Then my raw XML content should be equivalent to the resource called "HBaseXml_Set_HelloWorld"

Scenario: Parse XML for a set of cells
	Given I have raw content equal to the resource called "HBaseXml_Set_HelloWorld"
	When I convert my raw content to a set of cells
	Then my set should contain 4 cells
	And the cells in my set should have the following properties:
		| row | column | qualifier | timestamp | value       |
		| 1   | alpha  |           |           | hello world |
		| 1   | alpha  | x         |           | hello world |
		| 1   | alpha  |           | 4         | hello world |
		| 1   | alpha  | x         | 4         | hello world |