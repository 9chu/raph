## raph

某软件学院的编译原理大作业（[作业要求](http://wenku.baidu.com/view/3b92aa106edb6f1aff001fa7.html "作业要求")）。

目的为实现一个代码绘图语言。

鉴于题目要求过于简略，于是在此基础上脑补出一套实现。

本项目使用C#(.net 4.0)实现，raph由BR命名，取自单词graph。

仅限学习交流之用， **极度不推荐用于交作业** 。

### 样例

```
    origin is (100, 100);  // 移动绘图原点
    scale is (100, 100/3); // 缩放
    rot is pi/2;  // 旋转

    -- 绘图
    for count from 1 to 3 begin
        rot is rot + pi / 3;
        for ang from 0 to 2*pi step pi/80
            draw(cos(ang), sin(ang));
    end
```

### 词法元素

- 关键词：

```
    is begin end if else while for from to step true false while
```

- 运算符：

```
    + - * / ** > < >= <= == != && || = !
```

- 分隔符：

```
    ; ( ) { } ,
```

- 注释：

```
    -- //
```

### 语法范式

```
    block := begin statement_list end | statement
    statement_list := statement_list statement | ε
    statement := assignment | call | for_statement | while_statement | if_statement
    initialization := <identifier> is <expression>;
    assignment := <identifier> = <expression>;
    call := <identifier> ( <arg_list> );
    for_statement := for <identifier> from <expression> to <expression> step <expression> <block> | for <identifier> from <expression> to <expression> <block>
    if_statement := if <expression> <block> else <block> | if <expression> <block>
    while_statement := while <expression> <block>
    expression := <logic_expression>
    logic_expression := <logic_expression> && <relation_expression> | <logic_expression> || <relation_expression>
    relation_expression := <relation_expression> > <assign_expression> | <relation_expression> < <assign_expression> | 
        <relation_expression> >= <assign_expression> | <relation_expression> <= <assign_expression> | 
        <relation_expression> == <assign_expression> | <relation_expression> != <assign_expression> | <assign_expression>
    assign_expression := <assign_expression> = <plusminus_expression> | <plusminus_expression>
    plusminus_expression := <plusminus_expression> + <term_expression> | <plusminus_expression> - <term_expression> | <term_expression>
    term_expression := <term_expression> * <sub_expression> | <term_expression> / <sub_expression> | <power_expression>
    power_expression := <sub_expression> ** <unary_expression> | <unary_expression>
    unary_expression := - <atom_expression> | <atom_expression>
    atom_expression :=  ( <bracket_expression> ) | <call_expression> | <digit_literal> | <identifier> | <string_literal> | true | false
    call_expression := <identifier> ( <arg_list> )
    arg_list := arg_list, expression | expression
    bracket_expression := bracket_expression , expression | expression
```

### 运行时

- 预定义常量：

```
    PI = Math.PI
    E = Math.E
```

- 预定义数学函数：

```
    dot(vec2) -> digit
    cross(vec2, vec2) -> digit
    sin(digit) -> digit
    cos(digit) -> digit
    tan(digit) -> digit
    sinh(digit) -> digit
    cosh(digit) -> digit
    tanh(digit) -> digit
    asin(digit) -> digit
    acos(digit) -> digit
    atan(digit) -> digit
    atan2(digit, digit) -> digit
    sqrt(digit) -> digit
    exp(digit) -> digit
    ln(digit) -> digit
    log2(digit) -> digit
    log10(digit) -> digit
    abs(digit) -> digit
    max(digit, digit) -> digit
    min(digit, digit) -> digit
    ceil(digit) -> digit
    floor(digit) -> digit
    round(digit) -> digit
    trunc(digit) -> digit
    sgn(digit) -> digit
```

### 绘图运行时说明

- 特化常量

```
    ORIGIN = (0, 0)
    ROT = 0
    SCALE = (1, 1)
```

- 特化函数

```
    draw
    setPixelA
    setPixelR
    setPixelG
    setPixelB
```

- 变换顺序：比例 -> 旋转 -> 平移

### 许可

本项目基于MIT许可，详细信息见license.txt
